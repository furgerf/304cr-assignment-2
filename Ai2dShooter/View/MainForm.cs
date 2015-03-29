using System.Windows.Forms;
using Ai2dShooter.Map;
using Ai2dShooter.Model;

namespace Ai2dShooter.View
{
    public partial class MainForm : Form
    {
        #region Fields

        private readonly Player[] _players = {new Player(), new Player(), new Player(), new Player()};

        private readonly PlayerControl[] _playerControls;

        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();

            Maze.CreateNew(30, 15);
            Maze.Instance.SaveMap("maze with path.png");

            _playerControls = new[] {playerControl1, playerControl2, playerControl3, playerControl4};

            for (var i = 0; i < _players.Length; i++)
                _playerControls[i].Player = _players[i];

            _canvas.Paint += DrawCanvas;
        }

        #endregion

        #region Methods

        private void DrawCanvas(object sender, PaintEventArgs e)
        {
            Maze.DrawMaze(e.Graphics, 32);
        }

        #endregion

        #region Event Handling

        #endregion
    }
}
