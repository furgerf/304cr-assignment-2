using System;
using System.Linq;
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

        private Player HumanPlayer { get { return _players.FirstOrDefault(p => p.Controller == PlayerController.Human);}}

        private bool HasHumanPlayer { get { return HumanPlayer != null; } }

        /// <summary>
        /// Disables flickering when drawing on canvas.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;

                return cp;
            }
        }

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
            var playerControls = new[] {playerControl1, playerControl2, playerControl3, playerControl4, playerControl5, playerControl6};
            for (var i = 0; i < _players.Length; i++)
            {
                playerControls[i].Player = _players[i];
                _players[i].LocationChanged += (c, d) => _canvas.Invalidate();
            }

            // setup drawing
            _canvas.Paint += DrawCanvas;

            // setup key listening
            KeyUp += OnKeyUp;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets double buffering for flicker-less drawing.
        /// </summary>
        /// <param name="e">unused (here)</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DoubleBuffered = true;
        }

        private void DrawCanvas(object sender, PaintEventArgs e)
        {
            Maze.DrawMaze(e.Graphics, Utils.ScaleFactor);
            foreach (var p in _players)
                p.DrawPlayer(e.Graphics, Utils.ScaleFactor);
        }

        #endregion

        #region Event Handling

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!HasHumanPlayer)
                return;

            if (e.KeyCode == Keys.Up && HumanPlayer.CanMove(Direction.North))
                HumanPlayer.Move(Direction.North);
            if (e.KeyCode == Keys.Right && HumanPlayer.CanMove(Direction.East))
                HumanPlayer.Move(Direction.East);
            if (e.KeyCode == Keys.Down && HumanPlayer.CanMove(Direction.South))
                HumanPlayer.Move(Direction.South);
            if (e.KeyCode == Keys.Left && HumanPlayer.CanMove(Direction.West))
                HumanPlayer.Move(Direction.West);
        }

        #endregion
    }
}
