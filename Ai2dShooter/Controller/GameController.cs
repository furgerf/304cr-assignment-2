using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

                // add opponents
                _opponents.Add(p, players.Where(pp => pp.Team != p.Team).ToArray());
            }
        }

        #endregion

        #region Main Methods

        public void StartGame()
        {
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

                    // check whether the other player can see us too
                    if (opponent.Location.GetDirection(player.Location) == opponent.Orientation)
                        opponent.Damage(player, player.FrontDamage * (Constants.Rnd.Next() < player.HeadshotChance ? 2 : 1));
                    else
                        opponent.Damage(player, player.BackDamage * (Constants.Rnd.Next() < player.HeadshotChance ? 2 : 1));
                }
            }
        }

        #endregion
    }
}
