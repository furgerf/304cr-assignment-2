using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Map;
using Ai2dShooter.Model;
using Ai2dShooter.View;

namespace Ai2dShooter.Controller
{
    public class GameController
    {
        #region Public Fields

        public static GameController Instance { get; private set; }

        public bool GameRunning;

        public bool ArePlayersShooting { get { return _shootingPlayers != null; } }

        #endregion

        #region Private Fields

        private readonly Player[] _players;

        private readonly Dictionary<Player, Player[]> _opponents = new Dictionary<Player, Player[]>();

        private Player[] _shootingPlayers;

        private int _deathCount;

        #endregion

        #region Constructor

        public GameController(Player[] players)
        {
            Instance = this;

            _players = players;

            foreach (var p in players)
            {
                // register to events
                var p1 = p;
                p.LocationChanged += () => PlayerLocationChanged(p1);

                p.Death += () =>
                {
                    // remove from shooting player list
                    //var shooters = _shootingPlayers.First(s => s.Contains(p));
                    //_shootingPlayers.Remove(_shootingPlayers.First(s => s.Contains(p)));
                    //shooters.First(s => s != p).TriggerLocationChange();
                    _shootingPlayers = null;

                    // remove from opponent's opponent list
                    foreach (var opponent in _opponents[p1])
                        _opponents[opponent] = _opponents[opponent].Except(new[] {p1}).ToArray();

                    // remove own opponent list
                    _opponents.Remove(p1);

                    if (_deathCount++ == 0)
                        Constants.FirstBloodSound.Play();

                    foreach (var o in _opponents.Values)
                        if (o.Length == 0)
                        {
                            // game over!
                            // did all players of one team survive?
                            if (_opponents[_opponents.Keys.ToArray()[0]].Length == _opponents.Keys.Count)
                                Constants.PerfectSound.Play();

                            MainForm.ApplicationRunning = false;
                            StopGame();
                        }
                };

                // add opponents
                _opponents.Add(p, players.Where(pp => pp.Team != p.Team).ToArray());
            }
        }

        #endregion

        #region Main Methods

        public void StartGame()
        {
            Constants.PlaySound.Play();

            GameRunning = true;
            new Thread(() =>
            {
                foreach (var player in _players)
                {
                    // use random timeout between players starting
                    Thread.Sleep(Constants.Rnd.Next(Constants.AiMoveTimeout));
                    player.StartGame();
                }
            }).Start();
        }

        public void StopGame()
        {
            GameRunning = false;
        }

        public Cell GetClosestOpponentCell(Player player)
        {
            Cell closest = null;
            var visibleCells = player.VisibleReachableCells.ToArray();

            foreach (var o in _opponents[player])
            {
                var distance = player.Location.GetManhattenDistance(o.Location);
                if (!visibleCells.Contains(o.Location))
                    continue;

                closest = closest ?? o.Location;

                if (distance < player.Location.GetManhattenDistance(closest))
                    closest = o.Location;
            }
            return closest;
        }

        private bool IsShootingPlayer(Player player)
        {
            return _shootingPlayers != null && _shootingPlayers.Any(s => s == player);
        }

        public void CheckForOpponents(Player player)
        {
            if (IsShootingPlayer(player))
                return;
            //lock (Constants.HumanMovementLock)
                lock (Constants.MovementLock)
                {
                    // check whether player can shoot at any other player
                    foreach (var opponent in _opponents[player].Where(o => !IsShootingPlayer(o)))
                    {
                        if (player.Location.Neighbors.Contains(opponent.Location))
                        {
                            var op = opponent;
                            // *someone* has spotted *someone*
                            // did the player move into a camper's crosshair?
                            if (opponent.Location.GetNeighbor(opponent.Orientation) == player.Location)
                            {
                                Console.WriteLine(player + " got camped by " + opponent + "!");
                                // swap player and opponent
                                op = player;
                                player = opponent;
                            }

                            Console.WriteLine(player + " has spotted " + op);
                            player.EnemySpotted();
                            op.SpottedByEnemy();

                            if (_shootingPlayers != null)
                                throw new Exception("Can only have one gunfight at a time!");
                            _shootingPlayers = new[] {player, op};

                            var hit = Constants.Rnd.NextDouble() < player.ShootingAccuracy;
                            var headshot = hit && (Constants.Rnd.NextDouble() < player.HeadshotChance);
                            var frontalAttack = op.Location.GetDirection(player.Location) == op.Orientation;

                            Thread.Sleep(Constants.ShootingTimeout);
                            op.Damage(player,
                                (hit ? 1 : 0)*(frontalAttack ? player.FrontDamage : player.BackDamage)*
                                (headshot ? 2 : 1),
                                frontalAttack, headshot);
                            return;
                        }
                    }
                }
        }

        #endregion

        #region Event Handling

        private void PlayerLocationChanged(Player player)
        {
            if (IsShootingPlayer(player) || !player.IsAlive)
                return;

            CheckForOpponents(player);
        }

        #endregion
    }
}
