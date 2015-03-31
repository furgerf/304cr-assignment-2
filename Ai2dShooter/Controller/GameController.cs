using Ai2dShooter.Model;

namespace Ai2dShooter.Controller
{
    public class GameController
    {
        #region Private Fields

        private readonly Player[] _players;

        public static bool GameRunning;

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
            // check whether player can now see another player

            // if so, start shooting...
        }

        #endregion
    }
}
