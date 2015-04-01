using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Controller;
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

        public int Slowness { get; private set; }

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

                if (Health <= 0 && Death != null)
                {
                    Death();
                }
            }
        }

        public int HealthyThreshold { get; private set; }

        public int FrontDamage { get; private set; }

        public int BackDamage { get; private set; }

        public double HeadshotChance { get; private set; }

        public string Name { get; private set; }

        public Cell Location
        {
            get { return _location; }
            set
            {
                if (_location == value || !IsAlive) return;

                //var prev = _location;


                lock (Constants.MovementLock)
                {
                    //Constants.MovementSemaphore.WaitOne();
                    // update value
                    _location = value;
                    //Constants.MovementSemaphore.Release();
                }
                //Console.WriteLine(this + " moved from " + prev);

                // trigger event
                if (LocationChanged != null)
                    LocationChanged();
            }
        }

        public Color Color { get; private set; }

        public Teams Team { get; private set; }

        public PlayerController Controller { get; private set; }

        public Direction Orientation
        {
            get { return _orientation; }
            private set
            {
                if (!IsAlive) return;
                _orientation = value;
            }
        }

        #endregion

        #region Private Fields

        private static readonly string[] PlayerNames = Resources.names.Split('\n');

        private int _health;

        private Cell _location;

        private PointF _locationOffset;

        private Direction _orientation;

        protected bool IsMoving { get; set; }

        #endregion

        #region Constructor

        protected Player(Cell initialLocation, PlayerController controller, Teams team)
        {
            Slowness = Constants.Rnd.Next(300, 400);
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
                while (MainForm.ApplicationRunning)
                {
                    // zzzzzzzzZZZZZZZZZZzzzzzz
                    if (!IsMoving)
                    {
                        Thread.Sleep(Constants.Framerate);
                        continue;
                    }

                    // get offset in right direction
                    PointF stepOffset = Utils.GetDirectionPoint(Orientation);
                    // divide offset to get offset for single step
                    stepOffset = new PointF(stepOffset.X/Constants.Framerate, stepOffset.Y/Constants.Framerate);

                    // do half the steps
                    int i;
                    for (i = 0; i < Constants.Framerate && IsMoving && MainForm.ApplicationRunning && IsAlive; i++)
                    {
                        _locationOffset.X += stepOffset.X;
                        _locationOffset.Y += stepOffset.Y;
                        Thread.Sleep(Slowness/Constants.Framerate);
                    }

                    if (!IsMoving)
                    {
                        for (; i >= 0 && MainForm.ApplicationRunning && IsAlive; i-=2)
                        {
                            _locationOffset.X -= 2*stepOffset.X;
                            _locationOffset.Y -= 2*stepOffset.Y;
                            Thread.Sleep(Slowness / Constants.Framerate);
                        }

                        // clear offset
                        _locationOffset = Point.Empty;

                        Console.WriteLine(this + " had his movement aborted");
                        //ResumeMovementAfterShooting();
                        continue;
                    }

                    // clear offset
                    _locationOffset = Point.Empty;

                    // stop moving
                    IsMoving = false;

                    //Console.WriteLine("Changing location");
                    Location = Location.GetNeighbor(Orientation);
                }
            }).Start();
        }

        #endregion

        #region Implemented Methods

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
            if (Controller != PlayerController.Human && IsAlive)
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

            GameController.Instance.CheckForOpponents(this);

            // abort if we're already moving
            if (IsMoving)
                return;

            // assign to backing field because locationchanged will be triggered when updating location
            Orientation = direction;

            // tell movement thread to start moving
            IsMoving = true;
            //Console.WriteLine(this + " is moving towards " + Location.GetNeighbor(direction));
        }

        public void Damage(Player opponent, int damage, bool frontalAttack, bool headshot)
        {
            // reduce life
            Health -= damage <= Health ? damage : Health;

            Console.WriteLine(this + " has taken " + damage + " damage from " + opponent + " from " + (frontalAttack ? "the front" : "the back") + (headshot ? ", it was a HEADSHOT!" : ""));
            if (headshot)
            {
                Constants.HeadshotSound.Play();
            }
            else
            {
                if (damage > 55)
                    Constants.HardHitSound.Play();
                else if (damage > 45)
                    Constants.MediumHitSound.Play();
                else
                    Constants.LowHitSound.Play();
            }

            if (!Location.IsNeighbor(opponent.Location))
            {
                Console.WriteLine("The opponent got away...");

                //ResumeMovementAfterShooting();
                //opponent.ResumeMovementAfterShooting();

                return;
            }

            // retaliate!
            if (!IsAlive)
            {
                //Constants.MovementSemaphore.Release();
                Console.WriteLine(this + " has died!");

                // notify self of death
                //if (Death != null)
                //    Death();

                // notify opponent of death
                opponent.KilledEnemy();
                return;
            }

            // turn towards opponent
            Orientation = Location.GetDirection(opponent.Location);

            Thread.Sleep(Constants.ShootingTimeout);

            var hs = Constants.Rnd.NextDouble() < HeadshotChance;
            opponent.Damage(this, FrontDamage * (hs ? 2 : 1), true, hs);
        }

        #endregion

        #region Abstract Methods

        public abstract void StartGame();

        public abstract void EnemySpotted();

        public abstract void SpottedByEnemy();

        //protected abstract void ResumeMovementAfterShooting();

        public abstract void KilledEnemy();

        #endregion

        #region Event Handling



        #endregion

        public void TriggerLocationChange()
        {
            if (LocationChanged != null)
                LocationChanged();
        }
    }
}
