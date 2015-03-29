using System.Windows.Forms;
using Ai2dShooter.Common;
using Ai2dShooter.Map;
using Ai2dShooter.Model;

namespace Ai2dShooter.View
{
    public partial class MainForm : Form
    {
        #region Fields

        private readonly Player[] _players;

        private readonly PlayerControl[] _playerControls;

        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();

            // setup map
            Maze.CreateNew(30, 15);
            Maze.Instance.SaveMap("maze with path.png");

            // setup players
            _players = new[]
            {
                new Player(Maze.Instance.NorthWestCorner, PlayerController.Human, Teams.TeamHot),
                new Player(Maze.Instance.NorthCenterCorner, PlayerController.AiFsm, Teams.TeamHot),
                new Player(Maze.Instance.NorthEastCorner, PlayerController.AiFsm, Teams.TeamHot),
                new Player(Maze.Instance.SouthEastCorner, PlayerController.AiFsm, Teams.TeamCold),
                new Player(Maze.Instance.SouthCenterCorner, PlayerController.AiFsm, Teams.TeamCold),
                new Player(Maze.Instance.SouthWestCorner, PlayerController.AiFsm, Teams.TeamCold)
            };

            // setup player controls
            _playerControls = new[] {playerControl1, playerControl2, playerControl3, playerControl4, playerControl5, playerControl6};
            for (var i = 0; i < _players.Length; i++)
                _playerControls[i].Player = _players[i];

            // setup drawing
            _canvas.Paint += DrawCanvas;
        }

        #endregion

        #region Methods

        private void DrawCanvas(object sender, PaintEventArgs e)
        {
            Maze.DrawMaze(e.Graphics, Utils.ScaleFactor);
            foreach (var p in _players)
                p.DrawPlayer(e.Graphics, Utils.ScaleFactor);
        }

        #endregion

        #region Event Handling

        #endregion
    }
}
