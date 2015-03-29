using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ai2dShooter.Common
{
    public static class Utils
    {
        public static readonly Random Rnd = new Random();

        public const int ScaleFactor = 32;

        public static readonly Dictionary<PlayerController, string> PlayerControllerNames = new Dictionary<PlayerController, string> {{PlayerController.Human, "Human"}, {PlayerController.AiFsm, "AI: Finite State Machine"}};

        private static readonly Dictionary<Teams, Color[]> TeamColors = new Dictionary<Teams, Color[]>
        {
            {Teams.TeamHot, new []{Color.Gold, Color.LightCoral, Color.Magenta, Color.Maroon, Color.Purple, Color.SaddleBrown, Color.Red}},
            {Teams.TeamCold, new []{Color.RoyalBlue, Color.PowderBlue, Color.Navy, Color.Green, Color.GreenYellow,  Color.MediumSeaGreen, Color.Aqua}}
        };

        public static Color GetTeamColor(Teams team)
        {
            return TeamColors[team][Rnd.Next(TeamColors[team].Length)];
        }
    }
}
