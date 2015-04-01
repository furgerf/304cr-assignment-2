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

        private static Player[] _players;

        private static Player HumanPlayer { get { return _players.FirstOrDefault(p => p.Controller == PlayerController.Human);}}

        public static bool HasLivingHumanPlayer { get { return HumanPlayer != null && HumanPlayer.IsAlive; } }

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

        public static bool ApplicationRunning { get; set; }

        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();

            ApplicationRunning = true;

            // setup map
            Maze.CreateNew(30, 15); // 30, 15

            _canvas.Size = new Size(Constants.ScaleFactor * Maze.Instance.Width, Constants.ScaleFactor *Maze.Instance.Height);

            // setup players
            _players = new Player[]
            {
                new HumanPlayer(Maze.Instance.NorthWestCorner, Teams.TeamHot),
                //new FsmPlayer(Maze.Instance.NorthWestCorner, Teams.TeamHot), 
                new FsmPlayer(Maze.Instance.NorthWestCorner, Teams.TeamHot), 
                new FsmPlayer(Maze.Instance.NorthCenterCorner, Teams.TeamHot), 
                new FsmPlayer(Maze.Instance.NorthCenterCorner, Teams.TeamHot), 
                new FsmPlayer(Maze.Instance.NorthEastCorner, Teams.TeamHot),
                new FsmPlayer(Maze.Instance.NorthEastCorner, Teams.TeamHot),
                new FsmPlayer(Maze.Instance.SouthEastCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthEastCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthCenterCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthCenterCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthWestCorner, Teams.TeamCold),
                new FsmPlayer(Maze.Instance.SouthWestCorner, Teams.TeamCold)
            };

            // setup player controls
            var playerControlRight = 0;
            var playerControlBottom = 0;
            for (var i = 0; i < _players.Length; i++)
            {
                var pc = new PlayerControl {Player = _players[i]};
                pc.Location = new Point(_canvas.Location.X + (pc.Width + 8) * i, _canvas.Location.Y + _canvas.Height + 8);
                playerControlRight = pc.Right;
                playerControlBottom = pc.Bottom;

                Controls.Add(pc);
                _players[i].LocationChanged += () => _canvas.Invalidate();
            }
            Width = playerControlRight + 16;
            Height = playerControlBottom + 32;

            // setup drawing
            _canvas.Paint += DrawCanvas;

            // setup key listening
            KeyUp += OnKeyUp;

            // setup canvas invalidator thread
            new Thread(() =>
            {
                while (ApplicationRunning)
                {
                    _canvas.Invalidate();
                    Thread.Sleep(Constants.Framerate);
                }
                
            }).Start();

            new GameController(_players).StartGame();
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
            // draw map
            Maze.DrawMaze(e.Graphics, Constants.ScaleFactor);
            // draw dead players
            foreach (var p in _players.Where(p => !p.IsAlive))
                p.DrawPlayer(e.Graphics, Constants.ScaleFactor);
            // draw alive players
            foreach (var p in _players.Where(p => p.IsAlive))
                p.DrawPlayer(e.Graphics, Constants.ScaleFactor);
            // draw fog
            DrawFog(e.Graphics);
        }

        private void DrawFog(Graphics graphics)
        {
            if (!HasLivingHumanPlayer)
                return;

            for (var x = 0; x < Maze.Instance.Width; x++)
                for (var y = 0; y < Maze.Instance.Height; y++)
                    if (HumanPlayer.Location.GetManhattenDistance(x, y) > Constants.Visibility)
                        graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 32, 32, 32)),
                            new Rectangle(x*Constants.ScaleFactor, y*Constants.ScaleFactor,
                                Constants.ScaleFactor,
                                Constants.ScaleFactor));
        }

        #endregion

        #region Event Handling

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!HasLivingHumanPlayer || !HumanPlayer.IsAlive)
                return;

            new Thread(() => 
            {
                if (GameController.Instance.ArePlayersShooting)
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
            }).Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationRunning = false;
            GameController.Instance.StopGame();
        }

        #endregion
    }
}
