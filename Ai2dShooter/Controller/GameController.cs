using System;
using System.Collections.Generic;
using System.Linq;
using Ai2dShooter.Common;
using Ai2dShooter.Model;

namespace Ai2dShooter.Controller
{
    public class GameController
    {
        #region Private Fields

        private readonly Player[] _players;

        private readonly Dictionary<Player, Player[]> _opponents = new Dictionary<Player, Player[]>();
        
        public static bool GameRunning;
 

        #endregion

        #region Constructor

        public GameController(Player[] players)
        {
            _players = players;

            foreach (var p in players)
            {
                // register to events
                var p1 = p;
                p.LocationChanged += () => PlayerLocationChanged(p1);

                p.Death += () =>
                {
                    // remove from opponent's opponent list
                    foreach (var opponent in _opponents[p1])
                        _opponents[opponent] = _opponents[opponent].Except(new[] {p1}).ToArray();

                    // remove own opponent list
                    _opponents.Remove(p1);
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
            foreach (var player in _players)
            {
                player.StartGame();
            }
        }

        public void StopGame()
        {
            GameRunning = false;
        }

        #endregion

        #region Event Handling

        private void PlayerLocationChanged(Player player)
        {
            // check whether player can shoot at any other player
            foreach (var opponent in _opponents[player])
            {
                if (player.Location.Neighbors.Contains(opponent.Location))
                {
                    Console.WriteLine(player + " has spotted " + opponent);

                    // if so, start shooting...
                    player.EnemySpotted();

                    var headshot = Constants.Rnd.NextDouble() < player.HeadshotChance;
                    var frontalAttack = opponent.Location.GetDirection(player.Location) == opponent.Orientation;

                    opponent.Damage(player, (frontalAttack ? player.FrontDamage : player.BackDamage) * (headshot ? 2 : 1), frontalAttack, headshot);
                }
            }
        }

        #endregion
    }
}
