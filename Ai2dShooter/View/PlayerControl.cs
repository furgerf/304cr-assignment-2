using System.Globalization;
using System.Windows.Forms;
using Ai2dShooter.Model;

namespace Ai2dShooter.View
{
    public partial class PlayerControl : UserControl
    {
        #region Fields

        public Player Player
        {
            get { return _player; }
            set
            {
                if (Player == value) return;

                if (Player != null)
                {
                    UnregisterEvents(Player);
                }

                _player = value;

                if (Player != null)
                {
                    UpdateControls();
                    RegisterEvents(Player);
                }
            }
        }

        private Player _player;

        private readonly Player.OnLocationChanged _updateLocation;

        private readonly Player.OnHealthChanged _updateHealth;

        #endregion

        #region Constructor

        public PlayerControl()
        {
            InitializeComponent();

            _updateLocation = (oldPosition, newPosition) => grpName.Text = Player.Name + " - " + Player.Location;
            _updateHealth = (oldHealth, newHealth) => progressHealth.Value = Player.Health;
        }

        #endregion

        #region Methods

        private void UpdateControls()
        {
            grpName.ForeColor = Player.Color;
            _updateLocation(null, null);
            _updateHealth(0, 0);
            txtHealthyThreshold.Text = Player.HealthyThreshold.ToString(CultureInfo.InvariantCulture);
            txtDamage.Text = Player.FrontDamage + "/" + Player.BackDamage;
            txtHeadshot.Text = Player.HeadshotChance*100 + "%";
        }

        private void RegisterEvents(Player player)
        {
            player.HealthChanged += _updateHealth;
            player.LocationChanged += _updateLocation;
        }

        private void UnregisterEvents(Player player)
        {
            player.HealthChanged -= _updateHealth;
            player.LocationChanged -= _updateLocation;
        }

        #endregion

        #region Event Handling
        
        #endregion
    }
}
