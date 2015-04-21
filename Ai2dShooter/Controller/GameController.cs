using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ai2dShooter.Common;
using Ai2dShooter.Map;
using Ai2dShooter.Model;
using Ai2dShooter.View;

namespace Ai2dShooter.Controller
{
    /// <summary>
    /// Class that keeps track of the current game.
    /// </summary>
    public sealed class GameController
    {
        #region Public Fields

        /// <summary>
        /// Currently running game. Updated whenever a new game is created.
        /// </summary>
        public static GameController Instance { get; private set; }

        public static bool HasGame { get { return Instance != null; }}

        /// <summary>
        /// True if there is a game that is running or could be resumed.
        /// </summary>
        public bool GameRunning;

        public bool GamePaused { get; private set; }

        public bool ArePlayersShooting
        {
            get { return _shootingPlayers != null; }
        }

        #endregion

        #region Private Fields

        private readonly Dictionary<Player, Player[]> _opponents = new Dictionary<Player, Player[]>();

        private readonly Dictionary<Player, Player[]> _friends = new Dictionary<Player, Player[]>();

        private Player[] _shootingPlayers;

        private int _deathCount;

        #endregion

        #region Constructor

        public GameController(Player[] players)
        {
            // update game instance
            Instance = this;

            // prepare game for the players
            foreach (var p in players)
            {
                // register to events
                var p1 = p;
                p.LocationChanged += () =>
                {
                    if (IsShootingPlayer(p1) || !p1.IsAlive)
                        return;

                    CheckForOpponents(p1);
                };

                p.Death += () =>
                {
                    // the players aren't shooting anymore
                    _shootingPlayers = null;

                    // remove from opponent's opponent list
                    foreach (var opponent in _opponents[p1])
                        _opponents[opponent] = _opponents[opponent].Except(new[] {p1}).ToArray();

                    // remove own opponent list
                    _opponents.Remove(p1);

                    // remove from friends' friend list
                    foreach (var friend in _friends[p1])
                        _friends[friend] = _friends[friend].Except(new[] {p1}).ToArray();

                    // remove own friend list
                    _friends.Remove(p1);

                    if (MainForm.Instance.PlaySoundEffects && _deathCount++ == 0)
                        MainForm.Instance.Invoke((MethodInvoker) (() => Constants.FirstBloodSound.Play()));

                    foreach (var o in _opponents.Values)
                        if (o.Length == 0)
                        {
                            // game over!
                            // did all players of one team survive?
                            if (MainForm.Instance.PlaySoundEffects && _opponents[_opponents.Keys.ToArray()[0]].Length == _opponents.Keys.Count)
                                Constants.PerfectSound.Play();

                            //MainForm.ApplicationRunning = false;
                            StopGame();
                        }
                };

                // add opponents
                _opponents.Add(p, players.Where(pp => pp.Team != p.Team).ToArray());

                // add friends
                _friends.Add(p, players.Where(pp => pp.Team == p.Team && pp != p).ToArray());
            }
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Starts a new game and tells the players to start.
        /// </summary>
        public void StartGame()
        {
            if (MainForm.Instance.PlaySoundEffects)
                Constants.PlaySound.Play();

            GamePaused = false;
            GameRunning = true;

            // start players on new thread so as not to block the main thread
            new Thread(() =>
            {
                foreach (var player in _friends.Keys)
                {
                    // use random timeout between players starting
                    Thread.Sleep(Constants.Rnd.Next(Constants.AiMoveTimeout));
                    player.StartGame();
                }
            }).Start();
        }

        /// <summary>
        /// Stop (pause) game.
        /// </summary>
        public void StopGame()
        {
            Instance = null;

            MainForm.Instance.StopGame();
        }

        /// <summary>
        /// Pauses or resumes the game.
        /// </summary>
        public void PauseResumeGame()
        {
            if (GamePaused)
            {
                GamePaused = false;
                return;
            }

            // set paused flag
            GamePaused = true;
            new Thread(() =>
            {
                // acquire proper lock to pause NOW (rather than SOON):
                // if players are shooting, stop them
                // else, stop players from moving
                var lockMe = ArePlayersShooting ? Constants.ShootingLock : Constants.MovementLock;
                lock (lockMe)
                {
                    while (GamePaused)
                        Thread.Sleep(100);
                }
            }).Start();
        }

        /// <summary>
        /// Returns the cell of the opponent that is closest to the player and is visible.
        /// </summary>
        /// <param name="player">Player looking for a target</param>
        /// <returns>Cell of the nearest target, or null if no target is visible</returns>
        public Cell GetClosestVisibleOpponentCell(Player player)
        {
            Cell closest = null;

            // get all visible cells
            var visibleCells = player.VisibleReachableCells.ToArray();

            // iterate over opponents to see whether they're close by
            foreach (var o in _opponents[player].Where(o => visibleCells.Contains(o.Location)))
            {
                // ensure closest isn't null
                closest = closest ?? o.Location;

                // if the current target is closer than the previously closest target, update it
                if (player.Location.GetManhattenDistance(o.Location) < player.Location.GetManhattenDistance(closest))
                    closest = o.Location;
            }

            // return target
            return closest;
        }

        /// <summary>
        /// Finds the closest friendly player.
        /// </summary>
        /// <param name="player">Player looking for friends</param>
        /// <returns>Closest friend or null if everyone else is dead</returns>
        public Player GetClosestFriend(Player player)
        {
            Player closest = null;

            // iterate over fiends
            foreach (var f in _friends[player].Where(f => f.FollowedPlayer != player))
            {
                // ensure closest isn't null for comparison below
                closest = closest ?? f;

                // update closest if the current friend is closer
                if (player.Location.GetManhattenDistance(f.Location) < player.Location.GetManhattenDistance(closest.Location))
                    closest = f;
            }

            // return closest friend
            return closest;
        }

        /// <summary>
        /// Determines whether the player is currently shooting
        /// </summary>
        /// <param name="player">Player that may be shooting</param>
        /// <returns>True if the player is shooting</returns>
        private bool IsShootingPlayer(Player player)
        {
            return _shootingPlayers != null && _shootingPlayers.Any(s => s == player);
        }

        /// <summary>
        /// Checks whether the player has any opponents in range (on a neighboring cell).
        /// </summary>
        /// <param name="player">Player that is looking for targets</param>
        public void CheckForOpponents(Player player)
        {
            if (IsShootingPlayer(player) || !player.IsAlive)
                return;

            lock (Constants.MovementLock)
            {
                if (!player.IsAlive || !_opponents.ContainsKey(player))
                    return;

                var opponents = _opponents[player].Where(o => !IsShootingPlayer(o));

                // check whether player can shoot at any other player
                foreach (var opponent in opponents)
                {
                    if (!player.IsAlive || !_opponents.ContainsKey(player))
                        return;

                    // check next opponent if the current opponent is no neighbor
                    if (!player.Location.Neighbors.Contains(opponent.Location)) continue;

                    var op = opponent;
                    // *someone* has spotted *someone*
                    // did the player move into a camper's crosshair?
                    if (opponent.Location.GetNeighbor(opponent.Orientation) == player.Location)
                    {
                        Console.WriteLine(player + " got camped by " + opponent + "!");

                        // swap player and opponent because campers have first strike
                        op = player;
                        player = opponent;
                    }

                    // tell the players who spotted whom
                    Console.WriteLine(player + " has spotted " + op);
                    player.EnemySpotted();
                    op.SpottedByEnemy();

                    // ensure we don't already have a shots exchange
                    if (_shootingPlayers != null)
                        throw new Exception("Can only have one gunfight at a time!");
                    _shootingPlayers = new[] {player, op};

                    // calculate stuff for the first shot
                    var hit = Constants.Rnd.NextDouble() < player.ShootingAccuracy;
                    var headshot = hit && (Constants.Rnd.NextDouble() < player.HeadshotChance);
                    var frontalAttack = op.Location.GetDirection(player.Location) == op.Orientation;
                    var knife = player.UsesKnife;

                    if (!knife)
                        player.Ammo--;

                    Thread.Sleep(Constants.ShootingTimeout);
                    op.Damage(player,
                        (int)((hit ? 1 : 0)*(frontalAttack ? player.FrontDamage : player.BackDamage)*
                        (headshot ? 2 : 1)*(knife ? 0.5 : 1)),
                        frontalAttack, headshot, knife);

                    // done!
                    return;
                }
            }
        }

        /// <summary>
        /// Returns the cell where the friends have the lowest influence (highest distance).
        /// </summary>
        /// <param name="cells">Cells where the player could move to</param>
        /// <param name="player">Player looking for a cell to move to</param>
        /// <returns>Cell where there is the least influence by the player's team.</returns>
        public Cell GetCellWithLowestInfluence(Cell[] cells, Player player)
        {
            // if no friends are alive, return random cell
            if (_friends[player].Length == 0)
                return cells[Constants.Rnd.Next(cells.Length)];

            // if there is only one option, return that
            if (_friends[player].Length == 1)
                return cells[0];

            // prepare variables
            var influence = new int[cells.Length];

            // loop over possibilities
            for (var i = 0; i < cells.Length; i++)
            {
                // loop over friends
                foreach (var p in _friends[player])
                {
                    // if the friend has any influence, add it
                    // influence is negative distance because high distance -> low influence
                    var distance = p.Location.GetManhattenDistance(cells[i]);
                    influence[i] += distance <= 2*Constants.Visibility ? -distance : 0;
                }
            }

            // find all cells where influence is minimal
            var minInfluence = influence.Min();
            var minInfluenceIndices = new List<int>();
            for (var i = 0; i < influence.Length; i++)
            {
                if (influence[i] == minInfluence)
                    minInfluenceIndices.Add(i);
            }

            // return random cell with lowest influence
            return cells[minInfluenceIndices[Constants.Rnd.Next(minInfluenceIndices.Count)]];
        }

        #endregion
    }
}
