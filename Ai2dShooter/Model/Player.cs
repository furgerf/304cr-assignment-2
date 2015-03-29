using System.IO;
using System.Reflection;
using Ai2dShooter.Common;
using Ai2dShooter.Properties;

namespace Ai2dShooter.Model
{
    public class Player
    {
        #region Public Fields

        public int Health { get; private set; }

        public int HealthyThreshold { get; private set; }

        public int FrontDamage { get; private set; }
        public int BackDamage { get; private set; }

        public double HeadshotChance { get; private set; }

        public string Name { get; private set; }

        private static readonly string[] PlayerNames = Resources.names.Split('\n');

        #endregion

        #region Constructor

        public Player()
        {
            Health = 100;
            HealthyThreshold = Utils.Rnd.Next(10, 50);
            BackDamage = Utils.Rnd.Next(25, 75);
            FrontDamage = Utils.Rnd.Next(25, BackDamage);
            HeadshotChance = ((double) Utils.Rnd.Next(1, 4))/10;
            Name = PlayerNames[Utils.Rnd.Next(PlayerNames.Length)];
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
