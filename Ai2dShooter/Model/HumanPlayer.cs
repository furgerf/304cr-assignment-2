using System.Drawing;
using Ai2dShooter.Common;
using Ai2dShooter.Map;

namespace Ai2dShooter.Model
{
    public class HumanPlayer : Player
    {
        #region Constructor

        public HumanPlayer(Cell initialLocation, Teams team) : base(initialLocation, PlayerController.Human, team)
        {
        }

        #endregion

        #region Methods

        protected override void DrawPlayerImplementation(Graphics graphics, int scaleFactor, Rectangle box)
        {
            // draw a white H in the middle of the circle
            graphics.DrawString("H", new Font(FontFamily.GenericSansSerif, 22), Brushes.White, box);
        }

        protected override void ResetPlayerImplementation()
        {
            // do nothing
        }

        public override void StartGame()
        {
            // do nothing
        }

        public override void EnemySpotted()
        {
            // do nothing
        }

        public override void SpottedByEnemy()
        {
            // do nothing
        }

        #endregion
    }
}
