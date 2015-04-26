using System.Windows.Forms;
using Ai2dShooter.Common;

namespace Ai2dShooter.View
{
    /// <summary>
    /// Lets the user control some game settings.
    /// </summary>
    public partial class GameControl : UserControl
    {
        #region Fields

        /// <summary>
        /// Retrieves the controller of the members of team hot.
        /// </summary>
        public PlayerController[] TeamHot
        {
            get
            {
                var players = new PlayerController[(int)numPlayerCountHot.Value];
                var startIndex = 0;

                if (comPlayerControllerHot.SelectedItem.ToString().Contains("Human"))
                {
                    players[0] = PlayerController.Human;
                    startIndex = 1;
                }

                for (var i = startIndex; i < players.Length; i++)
                    for (var j = (int)PlayerController.Human + 1; j < (int)PlayerController.Count; j++)
                        if (comPlayerControllerHot.SelectedItem.ToString().Contains(((PlayerController)j).ToString()))
                        {
                            players[i] = (PlayerController)j;
                            break;
                        }
                return players;
            }
        }

        /// <summary>
        /// Retrieves the controller of the members of team cold.
        /// </summary>
        public PlayerController[] TeamCold
        {
            get
            {
                var players = new PlayerController[(int) numPlayerCountCold.Value];

                for (var i = 0; i < players.Length; i++)
                    for (var j = (int) PlayerController.Human + 1; j < (int) PlayerController.Count; j++)
                        if (comPlayerControllerCold.SelectedItem.ToString().Contains(((PlayerController) j).ToString()))
                        {
                            players[i] = (PlayerController) j;
                            break;
                        }

                return players;
            }
        }

        /// <summary>
        /// True if sound effects are enabled.
        /// </summary>
        public bool SoundEffects { get { return chkSoundEffects.Checked; } }

        /// <summary>
        /// True if influence maps should be drawn.
        /// </summary>
        public bool InfluenceMaps { get { return chkInfluenceMap.Checked; } }

        #endregion

        #region Constructor

        public GameControl()
        {
            InitializeComponent();

            // add combobox entries
            for (var i = (int) PlayerController.Human + 1; i < (int) PlayerController.Count; i++)
            {
                comPlayerControllerHot.Items.Add((PlayerController)i);
                comPlayerControllerHot.Items.Add("Human + " + (PlayerController)i);
                comPlayerControllerCold.Items.Add((PlayerController)i);
            }
            
            // assign default values
            comPlayerControllerHot.SelectedIndex = 1;
            comPlayerControllerCold.SelectedIndex = 1;

            numPlayerCountHot.Value = 6;
            numPlayerCountCold.Value = 6;
        }

        #endregion
    }
}
