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
        }

        #endregion

        #region Main Methods

        public override string ToString()
        {
            return Name + " (" + Health + "/100)";
        }

        #endregion

        #region Event Handling



        #endregion
    }
}
