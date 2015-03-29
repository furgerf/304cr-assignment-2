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

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Cell[,] Cells { get; private set; }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Maze Instance { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor initializes instance variables.
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        private Maze(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new Cell[width,height];

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    Cells[x, y] = new Cell(x, y);

            Cells[0, 0].IsWall = false;
        }

        #endregion

        #region Methods

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
                    var direction = cellDirections[Utils.Rnd.Next(cellDirections.Count)];
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
                        var neihbor = cell.GetNeighbor(otherDirections[Utils.Rnd.Next(otherDirections.Length)]);
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

        public void SaveMap(String filename)
        {
            var bitmap = new Bitmap(2*Width + 2, 2*Height + 2);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawRectangle(Pens.Brown, 0, 0, 2*Width + 1, 2*Height + 1);
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (Cells[x, y].IsWall)
                            graphics.DrawRectangle(Pens.Black, 2*x + 1, 2*y + 1, 1, 1);
                    }
                }
            }
            bitmap.Save(filename);
        }

        #endregion
    }
}
