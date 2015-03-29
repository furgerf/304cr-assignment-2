using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ai2dShooter.Common
{
    public static class Utils
    {
        public static readonly Random Rnd = new Random();

        public const int ScaleFactor = 32;

        public const int Visibility = 5;

        public static readonly Dictionary<PlayerController, string> PlayerControllerNames = new Dictionary<PlayerController, string> {{PlayerController.Human, "Human"}, {PlayerController.AiFsm, "AI: FSM"}};

        private static readonly Dictionary<Teams, List<Color>> TeamColors = new Dictionary<Teams, List<Color>>
        {
            {Teams.TeamHot, new List<Color>{Color.LightCoral, Color.Magenta, Color.Maroon, Color.Orange, Color.SaddleBrown, Color.Red}},
            {Teams.TeamCold, new List<Color>{Color.RoyalBlue, Color.Navy, Color.Green, Color.GreenYellow,  Color.MediumSeaGreen, Color.Aqua}}
        };

        public static Color GetTeamColor(Teams team)
        {
            var color = TeamColors[team][Rnd.Next(TeamColors[team].Count)];
            TeamColors[team].Remove(color);
            return color;
        }
    }
}
