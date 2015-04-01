using System;
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

        /// <summary>
        /// Called when the player's kills changes.
        /// </summary>
        private readonly Player.OnKillsChanged _updateKills;

        #endregion

        #region Constructor

        public PlayerControl()
        {
            InitializeComponent();

            // assign event handlers
            _updateLocation = () =>
            {
                try
                {
                    if (InvokeRequired)
                        Invoke((MethodInvoker) (() => _updateLocation()));
                    else
                        grpName.Text = Player.Name + " - " + Player.Location + " - " +
                                       Constants.PlayerControllerNames[Player.Controller];
                }
                catch (ObjectDisposedException ode)
                {
                    Console.WriteLine(ode.Message);
                }
            };
            _updateHealth = () =>
            {

                if (InvokeRequired)
                    Invoke((MethodInvoker)(() => _updateHealth()));
                else
                    progressHealth.Value = Player.Health;
            };
            _updateKills = () =>
            {
                if (InvokeRequired)
                    Invoke((MethodInvoker)(() => _updateKills()));
                else
                    txtKills.Text = Player.Kills.ToString(CultureInfo.InvariantCulture);
            };
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
            txtAccuracy.Text = Player.ShootingAccuracy + "/" + Player.HeadshotChance;
            txtSlowness.Text = Player.Slowness + "ms/cell";
            _updateKills();
        }

        private void RegisterEvents(Player player)
        {
            player.HealthChanged += _updateHealth;
            player.LocationChanged += _updateLocation;
            player.KillsChanged += _updateKills;
        }

        private void UnregisterEvents(Player player)
        {
            player.HealthChanged -= _updateHealth;
            player.LocationChanged -= _updateLocation;
            player.KillsChanged -= _updateKills;
        }

        #endregion

        #region Event Handling
        
        #endregion
    }
}
