using System.Drawing;
using System.Linq;
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

        private static Color[] PlayerColors = {Color.Red, Color.Green, Color.Indigo, Color.Magenta, Color.Olive, Color.Sienna, Color.Teal, Color.Black, Color.DarkRed };

        private int _health;
        private Cell _location;

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

        #endregion

        #region Constructor

        public Player(Cell initialLocation)
        {
            Health = 100;
            HealthyThreshold = Utils.Rnd.Next(10, 50);
            BackDamage = Utils.Rnd.Next(35, 75);
            FrontDamage = Utils.Rnd.Next(35, BackDamage);
            HeadshotChance = ((double) Utils.Rnd.Next(2, 6))/20; // 10-25%
            Name = PlayerNames[Utils.Rnd.Next(PlayerNames.Length)];
            Location = initialLocation;
            Color = PlayerColors[Utils.Rnd.Next(PlayerColors.Length)];
            var remainingColors = PlayerColors.ToList();
            remainingColors.Remove(Color);
            PlayerColors = remainingColors.ToArray();
        }

        #endregion

        #region Main Methods

        public override string ToString()
        {
            return Name + " (" + Location + ")";
        }

        public void DrawPlayer(Graphics graphics, int scaleFactor)
        {
            graphics.FillEllipse(new SolidBrush(Color), Location.X * scaleFactor - 1, Location.Y * scaleFactor - 1, scaleFactor + 1, scaleFactor + 1);
        }

        #endregion

        #region Event Handling



        #endregion
    }
}
