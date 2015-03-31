using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ai2dShooter.Common;
using Ai2dShooter.Model;
using Ai2dShooter.View;

namespace Ai2dShooter.Controller
{
    public class GameController
    {
        #region Private Fields

        private readonly Player[] _players;

        private bool _gameRunning;

        #endregion

        #region Constructor

        public GameController(Player[] players)
        {
            _players = players;

            // register to events
            foreach (var p in players)
            {
                var p1 = p;
                p.LocationChanged += () => PlayerLocationChanged(p1);
            }
        }

        #endregion

        #region Main Methods

        public void StartGame()
        {
            _gameRunning = true;
        }

        #endregion

        #region Event Handling

        private void PlayerLocationChanged(Player player)
        {
            // check whether player can now see another player

            // if so, start shooting...
        }

        #endregion
    }
}
