using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Map;
using Ai2dShooter.Properties;
using Ai2dShooter.View;

namespace Ai2dShooter.Model
{
    public abstract class Player
    {
        #region Events

        public delegate void OnHealthChanged();
        public event OnHealthChanged HealthChanged;

        public delegate void OnLocationChanged();
        public event OnLocationChanged LocationChanged;

        public delegate void OnDeath();
        public event OnDeath Death;

        #endregion

        #region Public Fields

        public bool IsAlive { get { return Health > 0; } }

        public int Health
        {
            get { return _health; }
            private set
            {
                if (_health == value) return;
             
                // update value
                _health = value;
                
                // trigger event
                if (HealthChanged != null)
                    HealthChanged();
            }
        }

        public int HealthyThreshold { get; private set; }

        public int FrontDamage { get; private set; }
        public int BackDamage { get; private set; }

        public double HeadshotChance { get; private set; }

        public string Name { get; private set; }

        private static readonly string[] PlayerNames = Resources.names.Split('\n');

        private int _health;
        private Cell _location;
        private Direction _orientation;

        public Cell Location
        {
            get { return _location; }
            set
            {
                if (_location == value) return;

                // update value
                _location = value;

                // trigger event
                if (LocationChanged != null)
                    LocationChanged();
            }
        }

        private PointF _locationOffset;

        protected bool IsMoving;

        public Color Color { get; private set; }

        public Teams Team { get; private set; }

        public PlayerController Controller { get; private set; }

        public Direction Orientation
        {
            get { return _orientation; }
            private set
            {
                //if (_orientation == value) return;

                // update value
                _orientation = value;

                //// trigger event
                //if (LocationChanged != null)
                //    LocationChanged();
            }
        }

        #endregion

        #region Constructor

        protected Player(Cell initialLocation, PlayerController controller, Teams team)
        {
            Team = team;
            Health = 100;
            HealthyThreshold = Constants.Rnd.Next(10, 50);
            BackDamage = Constants.Rnd.Next(35, 75);
            FrontDamage = Constants.Rnd.Next(35, BackDamage);
            HeadshotChance = ((double) Constants.Rnd.Next(2, 6))/20; // 10-25%
            Name = PlayerNames[Constants.Rnd.Next(PlayerNames.Length)];
            Location = initialLocation;
            Color = Utils.GetTeamColor(team);
            Controller = controller;
            Orientation = (Direction)Constants.Rnd.Next((int)Direction.Count);

            // start movement thread
            new Thread(() =>
            {
                // loop while application is running
                while (MainForm.IsRunning)
                {
                    // zzzzzzzzZZZZZZZZZZzzzzzz
                    if (!IsMoving)
                    {
                        Thread.Sleep(Constants.Framerate);
                        continue;
                    }

                    // get offset in right direction
                    PointF stepOffset = Utils.GetDirectionPoint(_orientation);
                    // divide offset to get offset for single step
                    stepOffset = new PointF(stepOffset.X/Constants.Framerate, stepOffset.Y/Constants.Framerate);

                    // do half the steps
                    for (var i = 0; i < Constants.Framerate/2 && IsMoving; i++)
                    {
                        _locationOffset.X += stepOffset.X;
                        _locationOffset.Y += stepOffset.Y;
                        Thread.Sleep(Constants.MsPerCell/Constants.Framerate);
                    }

                    if (!IsMoving)
                    {
                        _locationOffset = PointF.Empty;
                        continue;
                    }

                    // move to next cell
                    _locationOffset.X = -_locationOffset.X;
                    _locationOffset.Y = -_locationOffset.Y;
                    _location = Location.GetNeighbor(_orientation);

                    // do other half of the steps
                    for (var i = 0; i < Constants.Framerate / 2 && IsMoving; i++)
                    {
                        _locationOffset.X += stepOffset.X;
                        _locationOffset.Y += stepOffset.Y;
                        Thread.Sleep(Constants.MsPerCell/Constants.Framerate);
                    }

                    if (!IsMoving)
                    {
                        _locationOffset = PointF.Empty;
                        continue;
                    }

                    // clear offset
                    _locationOffset = Point.Empty;

                    // stop moving
                    IsMoving = false;

                    // move is complete, trigger event
                    if (LocationChanged != null)
                        LocationChanged();
                }
            }).Start();
        }

        #endregion

        #region Main Methods

        protected void AbortMovement()
        {
            IsMoving = false;
        }

        public override string ToString()
        {
            return Name + " " + Location;
        }

        public void DrawPlayer(Graphics graphics, int scaleFactor)
        {
            // box in which to draw the opponent
            var box = new Rectangle((int)((Location.X+_locationOffset.X)*scaleFactor) - 1, (int)((Location.Y + _locationOffset.Y)*scaleFactor) - 1, scaleFactor + 1,
                scaleFactor + 1);

            // draw opponent circle
            graphics.FillEllipse(new SolidBrush(Color.FromArgb(IsAlive ? 255 : 64, Color)), box);

            // start of the orientation line
            var orientationStart = new Point(box.Left + box.Width/2, box.Top + box.Height/2);

            // get end of the orientation line (depending on orientation)
            Point orientationEnd;
            switch (Orientation)
            {
                case Direction.North:
                    orientationEnd = new Point((box.Left + box.Right) / 2, box.Top);
                    break;
                case Direction.East:
                    orientationEnd = new Point(box.Right, (box.Bottom + box.Top) / 2);
                    break;
                case Direction.South:
                    orientationEnd = new Point((box.Left + box.Right) / 2, box.Bottom);
                    break;
                case Direction.West:
                    orientationEnd = new Point(box.Left, (box.Bottom + box.Top) / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // draw orientation line
            graphics.DrawLine(new Pen(Color.FromArgb(IsAlive ? 255 : 64, Color.Black), 4), orientationStart, orientationEnd);

            // draw opponent visibility range
            if (Controller != PlayerController.Human)
            {
                for (var x = Location.X - Constants.Visibility < 0 ? 0 : Location.X - Constants.Visibility; x <= (Location.X + Constants.Visibility > Maze.Instance.Width - 1 ? Maze.Instance.Width - 1 : Location.X + Constants.Visibility); x++)
                    for (var y = Location.Y - Constants.Visibility < 0 ? 0 : Location.Y - Constants.Visibility; y <= (Location.Y + Constants.Visibility > Maze.Instance.Height - 1 ? Maze.Instance.Height - 1 : Location.Y + Constants.Visibility); y++)
                    {
                        if (Maze.Instance.Cells[x, y] == null)
                            continue;

                        if (Location.GetManhattenDistance(x, y) <= Constants.Visibility)
                            graphics.FillRectangle(new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(127, Color), Color.FromArgb(0)), 
                                new Rectangle(x*Constants.ScaleFactor, y*Constants.ScaleFactor,
                                    Constants.ScaleFactor,
                                    Constants.ScaleFactor));
                    }
            }
        }

        public bool CanMove(Direction direction)
        {
            var c = Location.GetNeighbor(direction);

            return c != null && c.IsClear;
        }

        public void Move(Direction direction)
        {
            if (!CanMove(direction))
                throw new ArgumentException("Illegal move in direction " + direction);

            // abort if we're already moving
            if (IsMoving)
                return;

            // assign to backing field because locationchanged will be triggered when updating location
            _orientation = direction;

            // tell movement thread to start moving
            IsMoving = true;

            Console.WriteLine(this + " is moving towards " + Location.GetNeighbor(direction));
        }

        public abstract void StartGame();

        public abstract void EnemySpotted();

        public abstract void KilledEnemy();

        public void Damage(Player opponent, int damage, bool frontalAttack, bool headshot)
        {
            if (damage > 65)
                Constants.HardHitSound.Play();
            else if (damage > 50)
                Constants.MediumHitSound.Play();
            else
                Constants.LowHitSound.Play();

            // reduce life
            Health -= damage <= Health ? damage : Health;

            // turn opponent towards "me"
            Orientation = Location.GetDirection(opponent.Location);

            Console.WriteLine(this + " has taken " + damage + " damage from " + opponent + " from " + (frontalAttack ? "the front" : "the back") + (headshot ? ", it was a HEADSHOT!" : ""));
            if (headshot)
                Constants.HeadshotSound.Play();

            // retaliate!
            if (Health == 0)
            {
                Console.WriteLine(this + " has died!");

                // notify self of death
                if (Death != null)
                    Death();
                
                // notify opponent of death
                opponent.KilledEnemy();
                return;
            }

            Thread.Sleep(Constants.AiMoveTimeout);

            var hs = Constants.Rnd.NextDouble() < HeadshotChance;
            opponent.Damage(this, FrontDamage * (hs ? 2 : 1), true, hs);
        }
        #endregion

        #region Event Handling



        #endregion
    }
}
