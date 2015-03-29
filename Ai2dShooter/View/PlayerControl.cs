using System.Globalization;
using System.Windows.Forms;
using Ai2dShooter.Model;

namespace Ai2dShooter.View
{
    public partial class PlayerControl : UserControl
    {
        private Player _player;

        #region Fields

        public Player Player
        {
            get { return _player; }
            set
            {
                _player = value;

                if (Player != null)
                    UpdateControls();
            }
        }

        #endregion

        #region Constructor

        public PlayerControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void UpdateControls()
        {
            grpName.Text = Player.Name;
            progressHealth.Value = Player.Health;
            txtHealthyThreshold.Text = Player.HealthyThreshold.ToString(CultureInfo.InvariantCulture);
            txtDamage.Text = Player.FrontDamage + "/" + Player.BackDamage;
            txtHeadshot.Text = Player.HeadshotChance*100 + "%";
        }

        #endregion

        #region Event Handling

        #endregion
    }
}
