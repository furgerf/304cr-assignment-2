using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Model;
using Ai2dShooter.View;

namespace Ai2dShooter.Controller
{
    public class GameController
    {
        #region Public Fields

        public static GameController Instance { get; private set; }

        public bool GameRunning;

        #endregion

        #region Private Fields

        private readonly Player[] _players;

        private readonly Dictionary<Player, Player[]> _opponents = new Dictionary<Player, Player[]>();
        
        private readonly List<Player[]> _shootingPlayers = new List<Player[]>();

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
                    _shootingPlayers.Remove(_shootingPlayers.First(s => s.Contains(p)));

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

        private bool IsShootingPlayer(Player player)
        {
            return _shootingPlayers.Any(s => s.Contains(player));
        }

        public void CheckForOpponents(Player player)
        {
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

                        _shootingPlayers.Add(new[] { player, op });

                        var headshot = Constants.Rnd.NextDouble() < player.HeadshotChance;
                        var frontalAttack = op.Location.GetDirection(player.Location) == op.Orientation;

                        op.Damage(player, (frontalAttack ? player.FrontDamage : player.BackDamage) * (headshot ? 2 : 1),
                            frontalAttack, headshot);
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
