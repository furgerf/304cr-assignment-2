using System;
using Ai2dShooter.Common;

namespace Ai2dShooter.Map
{
    public class Cell
    {
        #region Fields

        // actual data
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsWall { get; set; }

        // neighbors
        public Cell[] Neighbors { get { return new[] { East, South, West, North }; } }
        public Cell West { get { return X > 0 ? Maze.Instance.Cells[X - 1, Y] : null; } }
        public Cell East { get { return X < Maze.Instance.Width - 1 ? Maze.Instance.Cells[X + 1, Y] : null; } }
        public Cell North { get { return Y > 0 ? Maze.Instance.Cells[X, Y - 1] : null; } }
        public Cell South { get { return Y < Maze.Instance.Height - 1 ? Maze.Instance.Cells[X, Y + 1] : null; } }

        public bool IsClear { get { return !IsWall; } }

        #endregion

        #region Constructor

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            IsWall = true;  // initially, all cells are walls
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the neighbor in the given direction.
        /// </summary>
        /// <param name="direction">Direction of the neighbor</param>
        /// <returns>Neighbor in the direction</returns>
        public Cell GetNeighbor(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return North;
                case Direction.East:
                    return East;
                case Direction.South:
                    return South;
                case Direction.West:
                    return West;
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        /// <summary>
        /// Returns the direction in which the neighbor lies.
        /// </summary>
        /// <param name="neighbor">Neighboring cell</param>
        /// <returns>Direction of the neighbor</returns>
        public Direction GetDirection(Cell neighbor)
        {
            if (neighbor == East)
                return Direction.East;
            if (neighbor == South)
                return Direction.South;
            if (neighbor == West)
                return Direction.West;
            if (neighbor == North)
                return Direction.North;

            throw new ArgumentException(neighbor + " is no neighbor!");
        }

        public int GetManhattenDistance(Cell other)
        {
            return GetManhattenDistance(other.X, other.Y);
        }

        public int GetManhattenDistance(int x, int y)
        {
            return Math.Abs(x - X) + Math.Abs(y - Y);
        }

        public bool Equals(Cell other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Cell && Equals((Cell)obj);
        }

        public static bool operator ==(Cell a, Cell b)
        {
            // return true if both are null or the same instance
            if (ReferenceEquals(a, b))
                return true;
            
            // return false if one of them is null
            if (((object)a == null) || ((object)b == null))
                return false;

            // return true if they're equal
            return a.Equals(b);
        }

        public static bool operator !=(Cell a, Cell b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
                return X ^ Y;
        }

        public override string ToString()
        {
            return "(" + X + "/" + Y + ")";
        }

        #endregion
    }
}
