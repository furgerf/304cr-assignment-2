using System.Drawing;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Map;

namespace Ai2dShooter.Model
{
    /// <summary>
    /// Player that is controlled by a human.
    /// </summary>
    public sealed class HumanPlayer : Player
    {
        #region Fields

        public bool IsReloading { get; private set; }

        #endregion

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

        public void Reload()
        {
            if (Ammo == MaxAmmo)
                return;

            IsReloading = true;

            new Thread(() =>
            {
                for (var i = 2; i >= 0; i--)
                {
                    lock (Constants.MovementLock)
                    {
                        Constants.ReloadSounds[i].Play();
                    }
                    Thread.Sleep(Constants.ReloadTimeout);
                }

                Ammo = MaxAmmo;
                IsReloading = false;
            }).Start();
        }

        #endregion
    }
}
