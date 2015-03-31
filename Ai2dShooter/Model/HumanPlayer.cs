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

        public override void StartGame()
        {
            // do nothing
        }

        public override void EnemySpotted()
        {
            // do nothing
        }

        protected override void ResumeMovement()
        {
            // do nothing
        }

        public override void SpottedByEnemy()
        {
            // do nothing
        }

        public override void KilledEnemy()
        {
            // do nothing
        }

        #endregion
    }
}
