using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ai2dShooter.Common;

namespace Ai2dShooter.Map
{
    /// <summary>
    /// Represents the logical structure of the map.
    /// </summary>
    public class Maze
    {
        #region Fields

        /// <summary>
        /// Action that draws the maze using the graphics object provided and scaled by the scale value.
        /// </summary>
        public static Action<Graphics, int> DrawMaze = (g, scale) =>
        {
            for (var x = 0; x < Instance.Width; x++)
                for (var y = 0; y < Instance.Height; y++)
                    g.FillRectangle(Instance.Cells[x, y].IsWall ? Brushes.Black : Brushes.BurlyWood, scale * x, scale * y, scale, scale);
        };

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Cell[,] Cells { get; private set; }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Maze Instance { get; private set; }

        // corners
        public Cell NorthWestCorner
        {
            get
            {
                var offset = 0;
                while (true)
                {
                    for (var i = 0; i <= offset; i++)
                    {
                        var j = offset - i;
                        if (Cells[i, j].IsClear)
                            return Cells[i, j];
                    }
                    offset++;
                }
            }
        }
        public Cell NorthCenterCorner
        {
            get
            {
                var offset = 0;
                while (true)
                {
                    if (Cells[Width/2 + offset, 0].IsClear)
                        return Cells[Width/2 + offset, 0];
                    if (Cells[Width/2 - offset, 0].IsClear)
                        return Cells[Width/2 - offset, 0];
                    offset++;
                }
            }
        }
        public Cell NorthEastCorner
        {
            get
            {
                var offset = 0;
                while (true)
                {
                    for (var i = 0; i <= offset; i++)
                    {
                        var j = offset - i;
                        if (Cells[Width - 1 - i, j].IsClear)
                            return Cells[Width - 1 - i, j];
                    }
                    offset++;
                }
            }
        }
        public Cell SouthEastCorner
        {
            get
            {
                var offset = 0;
                while (true)
                {
                    for (var i = 0; i <= offset; i++)
                    {
                        var j = offset - i;
                        if (Cells[Width - 1 - i, Height - 1 - j].IsClear)
                            return Cells[Width - 1 - i, Height - 1 - j];
                    }
                    offset++;
                }
            }
        }
        public Cell SouthCenterCorner
        {
            get
            {
                var offset = 0;
                while (true)
                {
                    if (Cells[Width/2 + offset, Height - 1].IsClear)
                        return Cells[Width/2 + offset, Height - 1];
                    if (Cells[Width/2 - offset, Height - 1].IsClear)
                        return Cells[Width/2 - offset, Height - 1];
                    offset++;
                }
            }
        }
        public Cell SouthWestCorner
        {
            get
            {
                var offset = 0;
                while (true)
                {
                    for (var i = 0; i <= offset; i++)
                    {
                        var j = offset - i;
                        if (Cells[i, Height - 1 - j].IsClear)
                            return Cells[i, Height - 1 - j];
                    }
                    offset++;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor initializes instance variables.
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        private Maze(int width, int height)
        {
            // assign dimensions
            Width = width;
            Height = height;

            // initiate cells
            Cells = new Cell[width,height];
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    Cells[x, y] = new Cell(x, y);
            Cells[0, 0].IsWall = false;
        }

        #endregion

        #region Methods

        public Cell GetCorner(bool north, int number)
        {
            if (north)
            {
                switch (number%3)
                {
                    case 0:
                        return NorthWestCorner;
                    case 1:
                        return NorthCenterCorner;
                    case 2:
                        return NorthEastCorner;
                }
            }
            switch (number%3)
            {
                case 0:
                    return SouthWestCorner;
                case 1:
                    return SouthCenterCorner;
                case 2:
                    return SouthEastCorner;
            }

            throw new Exception("This won't ever be executed");
        }

        /// <summary>
        /// Public access to random maze creation
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        public static void CreateNew(int width, int height)
        {
            if (width < 0) throw new ArgumentOutOfRangeException("width");
            if (height < 0) throw new ArgumentOutOfRangeException("height");

            // create instance
            Instance = new Maze(width, height);

            // generate maze
            Instance.GenerateMaze();
        }

        /// <summary>
        /// Generates a new maze, that is: Marks passable cells.
        /// </summary>
        public void GenerateMaze()
        {
            // initialize variables
            var cellVisited = new bool[Width, Height];
            var openCells = new Stack<Cell>();
            var cellDirections = new List<Direction>();

            // prepare initial data
            openCells.Push(Cells[0, 0]);
            cellVisited[0, 0] = true;

            // loop over open cells to create passages
            while (openCells.Count > 0)
            {
                // we are currently in the top-most open cell
                var cell = openCells.Peek();

                // find unvisited neighboring cells
                if (cell.West != null && !cellVisited[cell.West.X, cell.West.Y])
                    cellDirections.Add(Direction.West);
                if (cell.East != null && !cellVisited[cell.East.X, cell.East.Y])
                    cellDirections.Add(Direction.East);
                if (cell.North != null && !cellVisited[cell.North.X, cell.North.Y])
                    cellDirections.Add(Direction.North);
                if (cell.South != null && !cellVisited[cell.South.X, cell.South.Y])
                    cellDirections.Add(Direction.South);

                // did we find any unvisited neighbors?
                if (cellDirections.Any())
                {
                    // pick random neighbor to move to
                    var direction = cellDirections[Constants.Rnd.Next(cellDirections.Count)];
                    var directionCell = cell.GetNeighbor(direction);

                    // move to neighbor
                    cellVisited[directionCell.X, directionCell.Y] = true;
                    openCells.Push(directionCell);

                    // neighbor no longer is a wall
                    directionCell.IsWall = false;

                    // get remaining neighbors
                    var otherDirections = cellDirections.Except(new[]{direction}).ToArray();
                    if (otherDirections.Length > 0)
                    {
                        // if we have more than one other neighbor, randomly mark one as visited
                        // to ensure it stays a wall
                        var neihbor = cell.GetNeighbor(otherDirections[Constants.Rnd.Next(otherDirections.Length)]);
                        cellVisited[neihbor.X, neihbor.Y] = true;
                    }
                }
                else
                {
                    // dead end: backtrack
                    openCells.Pop();
                }
                cellDirections.Clear();
            }
        }

        #endregion
    }
}
