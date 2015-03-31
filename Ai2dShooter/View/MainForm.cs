using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ai2dShooter.Common;
using Ai2dShooter.Controller;
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

        public static bool IsRunning { get; set; }

        private GameController _currentGame;

        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();

            IsRunning = true;

            // setup map
            Maze.CreateNew(15, 15); // 30, 15
            Maze.Instance.SaveMap("maze with path.png");

            // setup players
            _players = new Player[]
            {
                new HumanPlayer(Maze.Instance.NorthWestCorner, Teams.TeamHot),
                //new FsmPlayer(Maze.Instance.NorthWestCorner, Teams.TeamHot), 
                new FsmPlayer(Maze.Instance.NorthCenterCorner, Teams.TeamHot), 
                new FsmPlayer(Maze.Instance.NorthEastCorner, Teams.TeamHot),
                new FsmPlayer(Maze.Instance.SouthEastCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthCenterCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthWestCorner, Teams.TeamCold)
            };

            // setup player controls
            var playerControls = new[] {playerControl1, playerControl2, playerControl3, playerControl4, playerControl5, playerControl6};
            for (var i = 0; i < _players.Length; i++)
            {
                playerControls[i].Player = _players[i];
                _players[i].LocationChanged += () => _canvas.Invalidate();
            }

            // setup drawing
            _canvas.Paint += DrawCanvas;

            // setup key listening
            KeyUp += OnKeyUp;

            // setup canvas invalidator thread
            new Thread(() =>
            {
                while (IsRunning)
                {
                    _canvas.Invalidate();
                    Thread.Sleep(Constants.Framerate);
                }
                
            }).Start();

            _currentGame = new GameController(_players);
            _currentGame.StartGame();
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
            Maze.DrawMaze(e.Graphics, Constants.ScaleFactor);
            foreach (var p in _players)
                p.DrawPlayer(e.Graphics, Constants.ScaleFactor);
            DrawFog(e.Graphics);
        }

        private void DrawFog(Graphics graphics)
        {
            if (!HasHumanPlayer)
                return;

            for (var x = 0; x < Maze.Instance.Width; x++)
                for (var y = 0; y < Maze.Instance.Height; y++)
                    if (HumanPlayer.Location.GetManhattenDistance(x, y) > Constants.Visibility)
                        graphics.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)),
                            new Rectangle(x*Constants.ScaleFactor, y*Constants.ScaleFactor,
                                Constants.ScaleFactor,
                                Constants.ScaleFactor));
        }

        #endregion

        #region Event Handling

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!HasHumanPlayer || !HumanPlayer.IsAlive)
                return;

            // arrow keys/wasd for player movement
            if ((e.KeyCode == Keys.W || e.KeyCode == Keys.Up) && HumanPlayer.CanMove(Direction.North))
                HumanPlayer.Move(Direction.North);
            if ((e.KeyCode == Keys.D || e.KeyCode == Keys.Right) && HumanPlayer.CanMove(Direction.East))
                HumanPlayer.Move(Direction.East);
            if ((e.KeyCode == Keys.S || e.KeyCode == Keys.Down) && HumanPlayer.CanMove(Direction.South))
                HumanPlayer.Move(Direction.South);
            if ((e.KeyCode == Keys.A || e.KeyCode == Keys.Left) && HumanPlayer.CanMove(Direction.West))
                HumanPlayer.Move(Direction.West);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsRunning = false;
            _currentGame.StopGame();
        }

        #endregion
    }
}
