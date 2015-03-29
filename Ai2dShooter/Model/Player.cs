using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Ai2dShooter.Common;
using Ai2dShooter.Map;
using Ai2dShooter.Properties;

namespace Ai2dShooter.Model
{
    public class Player
    {
        #region Events

        public delegate void OnHealthChanged(int oldHealth, int newHealth);

        public event OnHealthChanged HealthChanged;

        public delegate void OnLocationChanged(Cell oldPosition, Cell newPosition);

        public event OnLocationChanged LocationChanged;

        #endregion

        #region Public Fields

        public int Health
        {
            get { return _health; }
            set
            {
                if (_health == value) return;
                var oldHealth = Health;
                _health = value;
                
                if (HealthChanged != null)
                    HealthChanged(oldHealth, Health);
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
                // TODO: Maybe some value validation?
                var oldLocation = Location;
                _location = value;

                if (LocationChanged != null)
                    LocationChanged(oldLocation, Location);
            }
        }

        public Color Color { get; private set; }

        public Teams Team { get; private set; }

        public PlayerController Controller { get; private set; }

        public Direction Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation == value) return;

                _orientation = value;

                if (LocationChanged != null)
                    LocationChanged(Location, Location);
            }
        }

        #endregion

        #region Constructor

        public Player(Cell initialLocation, PlayerController controller, Teams team)
        {
            Team = team;
            Health = 100;
            HealthyThreshold = Utils.Rnd.Next(10, 50);
            BackDamage = Utils.Rnd.Next(35, 75);
            FrontDamage = Utils.Rnd.Next(35, BackDamage);
            HeadshotChance = ((double) Utils.Rnd.Next(2, 6))/20; // 10-25%
            Name = PlayerNames[Utils.Rnd.Next(PlayerNames.Length)];
            Location = initialLocation;
            Color = Utils.GetTeamColor(team);
            Controller = controller;
            Orientation = (Direction)Utils.Rnd.Next((int)Direction.Count);
        }

        #endregion

        #region Main Methods

        public override string ToString()
        {
            return Name + " (" + Location + ")";
        }

        public void DrawPlayer(Graphics graphics, int scaleFactor)
        {
            var box = new Rectangle(Location.X*scaleFactor - 1, Location.Y*scaleFactor - 1, scaleFactor + 1,
                scaleFactor + 1);

            var orientationStart = new Point(box.Left + box.Width/2, box.Top + box.Height/2);
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

            graphics.FillEllipse(new SolidBrush(Color), box);
            graphics.DrawLine(new Pen(Color.Black, 4), orientationStart, orientationEnd);
        }

        #endregion

        #region Event Handling



        #endregion
    }
}
