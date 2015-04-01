using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ai2dShooter.Common
{
    public static class Utils
    {
        public static Point GetDirectionPoint(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(0, -1);
                case Direction.East:
                    return new Point(1, 0);
                case Direction.South:
                    return new Point(0, 1);
                case Direction.West:
                    return new Point(-1, 0);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        private static readonly Dictionary<Teams, List<Color>> TeamColors = new Dictionary<Teams, List<Color>>
        {
            {Teams.TeamHot, new List<Color>{Color.LightCoral, Color.Magenta, Color.Maroon, Color.Orange, Color.SaddleBrown, Color.Red}},
            {Teams.TeamCold, new List<Color>{Color.RoyalBlue, Color.Navy, Color.Green, Color.MediumSeaGreen, Color.DarkGreen, Color.Aqua}}
        };

        public static Color GetTeamColor(Teams team)
        {
            var color = TeamColors[team][Constants.Rnd.Next(TeamColors[team].Count)];
            TeamColors[team].Remove(color);
            return color;
        }
    }
}
