using System;
using System.Collections.Generic;
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
        #region Public Fields

        public bool ApplicationRunning { get; set; }

        public bool HasLivingHumanPlayer { get { return HumanPlayer != null && HumanPlayer.IsAlive; } }

        public bool PlaySoundEffects { get { return _gameControl.SoundEffects; } }

        public static MainForm Instance { get; private set; }

        #endregion

        #region Private Fields

        private Player[] _players;

        private HumanPlayer HumanPlayer { get { return _players == null ? null : (HumanPlayer)_players.FirstOrDefault(p => p.Controller == PlayerController.Human);}}

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

        private readonly GameControl _gameControl;

        private readonly List<Control> _playerControls = new List<Control>(); 

        #endregion

        #region Constructor

        public MainForm()
        {
            Instance = this;

            InitializeComponent();

            WindowState = FormWindowState.Maximized;

            ApplicationRunning = true;

            // setup map
            Maze.CreateNew(35, 12); // 30, 15

            _canvas.Size = new Size(Constants.ScaleFactor*Maze.Instance.Width,
                Constants.ScaleFactor*Maze.Instance.Height);

            _gameControl = new GameControl {Location = new Point(_canvas.Right + _canvas.Padding.Right, _canvas.Top)};
            Controls.Add(_gameControl);

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
                    Thread.Sleep(Constants.Framelength);
                }
            }).Start();
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

            if (_players == null)
                return;

            // draw dead players
            foreach (var p in _players.Where(p => !p.IsAlive))
                p.DrawPlayer(e.Graphics, Constants.ScaleFactor);
            // draw alive players that are enemies if human is playing
            foreach (var p in _players.Where(p => p.IsAlive && (!HasLivingHumanPlayer || p.Team != HumanPlayer.Team)))
                p.DrawPlayer(e.Graphics, Constants.ScaleFactor);
            // draw fog
            DrawFog(e.Graphics);
            // draw alive players that are friends if human is playing
            foreach (var p in _players.Where(p => p.IsAlive && (!HasLivingHumanPlayer || p.Team == HumanPlayer.Team)))
                p.DrawPlayer(e.Graphics, Constants.ScaleFactor);
            // draw paused
            if (GameController.Instance.GamePaused)
                DrawPaused(e.Graphics);
        }

        private static void DrawPaused(Graphics graphics)
        {
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(Constants.DeadAlpha, Color.DimGray)), 0, 0, Maze.Instance.Width * Constants.ScaleFactor, Maze.Instance.Height * Constants.ScaleFactor);
        }

        private void DrawFog(Graphics graphics)
        {
            if (!HasLivingHumanPlayer || !GameController.Instance.GameRunning)
                return;

            var visible = HumanPlayer.VisibleReachableCells.ToArray();

            for (var x = 0; x < Maze.Instance.Width; x++)
                for (var y = 0; y < Maze.Instance.Height; y++)
                    if (!visible.Contains(Maze.Instance.Cells[x, y]) && (Maze.Instance.Cells[x, y]).IsClear || !Maze.Instance.Cells[x, y].Neighbors.Any(visible.Contains))
                        graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 32, 32, 32)),
                            new Rectangle(x * Constants.ScaleFactor, y * Constants.ScaleFactor,
                                Constants.ScaleFactor,
                                Constants.ScaleFactor));
        }

        /// <summary>
        /// Creates a new game.
        /// </summary>
        private void CreateGame()
        {
            _gameControl.Enabled = false;

            // get player controllers
            var hot = _gameControl.TeamHot;
            var cold = _gameControl.TeamCold;
            var allList = hot.ToList();
            allList.AddRange(cold);
            var all = allList.ToArray();

            Utils.ResetTeamColors();

            // create players
            _players = new Player[all.Length];
            for (var i = 0; i < all.Length; i++)
            {
                var isHot = i < hot.Length;
                switch (all[i])
                {
                    case PlayerController.Human:
                        _players[i] = new HumanPlayer(Maze.Instance.GetCorner(isHot, i),
                            isHot ? Teams.TeamHot : Teams.TeamCold);
                        break;
                    case PlayerController.AiFsm:
                        _players[i] = new FsmPlayer(Maze.Instance.GetCorner(isHot, i),
                            isHot ? Teams.TeamHot : Teams.TeamCold);
                        break;
                    case PlayerController.AiDt:
                        _players[i] = new DtPlayer(Maze.Instance.GetCorner(isHot, i),
                            isHot ? Teams.TeamHot : Teams.TeamCold);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // setup player controls
            for (var i = 0; i < _players.Length; i++)
            {
                var pc = new PlayerControl { Player = _players[i] };

                pc.Location = i < hot.Length ? new Point(_canvas.Location.X + (pc.Width + 8) * i, _canvas.Location.Y + _canvas.Height + 8) : new Point(_canvas.Location.X + (pc.Width + 8) * (i - hot.Length), _canvas.Location.Y + _canvas.Height + 8 + pc.Height + 8);

                _playerControls.Add(pc);
                _players[i].LocationChanged += () => _canvas.Invalidate();
            }
            Controls.AddRange(_playerControls.ToArray());

            // start new game
            new GameController(_players).StartGame();
        }

        /// <summary>
        /// Stops the current game.
        /// </summary>
        public void StopGame()
        {
            Instance.Invoke((MethodInvoker) (() => _gameControl.Enabled = true));

            if (GameController.Instance != null && GameController.Instance.GameRunning)
                GameController.Instance.StopGame();

            foreach (var pc in _playerControls)
            {
                var pc1 = pc;
                Instance.Invoke((MethodInvoker) pc1.Dispose);
            }
            _playerControls.Clear();

            if (_players != null)
                foreach (var p in _players)
                    p.RemovePlayer();

            _players = null;
        }

        #endregion

        #region Event Handling

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // space bar pauses/resumes game
            if (e.KeyCode == Keys.Space)
            {
                if (GameController.Instance == null)
                    return;

                GameController.Instance.PauseResumeGame();
                return;
            }

            // enter starts/stops game
            if (e.KeyCode == Keys.Enter)
            {
                if (GameController.HasGame)
                    StopGame();
                else
                    CreateGame();
                return;
            }

            // human-controlled player related commands
            if (!HasLivingHumanPlayer || !HumanPlayer.IsAlive)
                return;

            new Thread(() => 
            {
                // don't allow the human player to move while there is shooting going on
                if (GameController.Instance.ArePlayersShooting)
                    return;

                if (HumanPlayer.IsReloading)
                    return;

                if (e.KeyCode == Keys.R)
                    HumanPlayer.Reload();

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
            if (GameController.Instance != null && GameController.Instance.GamePaused)
                GameController.Instance.PauseResumeGame();

            lock (Constants.ShootingLock)
            {
                // tell the threads to terminate
                ApplicationRunning = false;
                
                if (GameController.Instance != null)
                    GameController.Instance.StopGame();
            }
        }

        #endregion
    }
}
