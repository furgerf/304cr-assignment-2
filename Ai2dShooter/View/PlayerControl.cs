using System.Globalization;
using System.Windows.Forms;
using Ai2dShooter.Common;
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

                // unregister events of old player
                if (Player != null)
                    UnregisterEvents(Player);

                // update value
                _player = value;

                // register events of new player
                if (Player != null)
                {
                    UpdateControls();
                    RegisterEvents(Player);
                }
            }
        }

        private Player _player;

        /// <summary>
        /// Called when the player's location changes.
        /// </summary>
        private readonly Player.OnLocationChanged _updateLocation;

        /// <summary>
        /// Called when the player's health changes.
        /// </summary>
        private readonly Player.OnHealthChanged _updateHealth;

        #endregion

        #region Constructor

        public PlayerControl()
        {
            InitializeComponent();

            // assign event handlers
            _updateLocation = () => grpName.Text = Player.Name + " - " + Player.Location + " - " + Constants.PlayerControllerNames[Player.Controller];
            _updateHealth = () => progressHealth.Value = Player.Health;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the controls to reflect the player's current status.
        /// </summary>
        private void UpdateControls()
        {
            grpName.ForeColor = Player.Color;
            _updateLocation();
            _updateHealth();
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
