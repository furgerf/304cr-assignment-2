using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Controller;
using Ai2dShooter.Map;

namespace Ai2dShooter.Model
{
    /// <summary>
    /// An AI player controlled by a decision tree (DT).
    /// </summary>
    public sealed class DtPlayer : Player
    {
        #region Private Fields

        /// <summary>
        /// Possible decisions that can be made.
        /// </summary>
        public enum DecisionType { MoveToEnemy, MoveToFriend, RandomMove, Reload, Count }


        private static readonly Color[] DecisionColors =
        {
            Color.Goldenrod, Color.Pink, Color.Green, Color.Gray
        };

        private readonly Pen _targetPen = new Pen(Color.FromArgb(127, Color.LawnGreen), 4);

        private Cell _targetCell;

        private DecisionType _lastDecision = DecisionType.Count;

        private bool _inCombat;

        private static readonly Decision Tree;

        #endregion

        #region Constructor

        static DtPlayer()
        {
            var data = ParseTreeCreationData(new[]
            {
                new DecisionData(true, true, true, DecisionType.MoveToEnemy),
                new DecisionData(true, true, false, DecisionType.RandomMove),
                new DecisionData(true, false, true, DecisionType.MoveToEnemy),
                new DecisionData(false, true, true, DecisionType.MoveToEnemy),
                new DecisionData(true, false, false, DecisionType.Reload),
                new DecisionData(false, true, false, DecisionType.MoveToFriend),
                new DecisionData(false, false, true, DecisionType.MoveToEnemy),
                new DecisionData(false, false, false, DecisionType.Reload)
            });

            Tree = Decision.CreateTree(data.Item1, data.Item2, data.Item3);
        }

        public DtPlayer(Cell initialLocation, Teams team)
            : base(initialLocation, PlayerController.AiFsm, team)
        {
            LocationChanged += MakeDecision;
            HealthChanged += MakeDecision;
            Death += () => _lastDecision = DecisionType.Count;
        }

        #endregion

        #region Main Methods

        protected override void DrawPlayerImplementation(Graphics graphics, int scaleFactor, Rectangle box)
        {
            // make the box a bit smaller (so the circle doesn't exceed the box)
            var smallerBox = box;
            const int penWidth = 4;
            smallerBox.Inflate(-penWidth, -penWidth);

            // draw circle in the color belonging to the current state
            if (_lastDecision != DecisionType.Count)
            {
                var edges = new[]
                {
                    // eight-side polygon
                    //new PointF(smallerBox.Left + (float)smallerBox.Width/3, smallerBox.Top),
                    //new PointF(smallerBox.Left + (float)smallerBox.Width*2/3, smallerBox.Top),
                    //new PointF(smallerBox.Right, smallerBox.Top + (float)smallerBox.Height/3),
                    //new PointF(smallerBox.Right, smallerBox.Top + (float)smallerBox.Height*2/3),
                    //new PointF(smallerBox.Left + (float)smallerBox.Width*2/3, smallerBox.Bottom),
                    //new PointF(smallerBox.Left + (float)smallerBox.Width/3, smallerBox.Bottom),
                    //new PointF(smallerBox.Left, smallerBox.Top + (float)smallerBox.Height*2/3),
                    //new PointF(smallerBox.Left, smallerBox.Top + (float)smallerBox.Height/3)

                    // diamond polygon
                    new PointF((float)(smallerBox.Left + smallerBox.Right) / 2, smallerBox.Top), 
                    new PointF(smallerBox.Right, (float)(smallerBox.Top + smallerBox.Bottom) / 2), 
                    new PointF((float)(smallerBox.Left + smallerBox.Right) / 2, smallerBox.Bottom), 
                    new PointF(smallerBox.Left, (float)(smallerBox.Top + smallerBox.Bottom) / 2)
                };
                graphics.DrawPolygon(new Pen(DecisionColors[(int)_lastDecision], penWidth), edges);
            }

            // if required, draw a line to the cell that is currently being targeted by the player
            if (_targetCell != null && IsAlive)
                graphics.DrawLine(_targetPen, box.X + box.Width/2, box.Y + box.Height/2,
                    _targetCell.X*scaleFactor + box.Width/2, _targetCell.Y*scaleFactor + box.Height/2);
        }

        public override void StartGame()
        {
            // start own worker thread
            new Thread(MakeDecision).Start();
        }

        public override void EnemySpotted()
        {
            _inCombat = true;

            // stop moving and start fighting
            AbortMovement();
        }

        public override void SpottedByEnemy()
        {
            _inCombat = true;

            // stop moving and start fighting
            AbortMovement();
        }

        public override void KilledEnemy()
        {
            base.KilledEnemy();

            _inCombat = false;

            // start movement
            MakeDecision();
        }

        private void MakeDecision()
        {
            if (IsMoving || !IsAlive || _inCombat || !PlayerExists)
                return;

            var neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();

            _lastDecision = DecisionType.Count;

            // stuck?
            if (neighbors.Length == 1)
            {
                //_lastDecision = DecisionType.Backtrack;

                // backtrack
                Move(Location.GetDirection(neighbors[0]));
            }
            else
            {
                // healthy?
                if (Health < HealthyThreshold)
                {
                    if (GameController.Instance == null)
                        return;

                    // find closest friend
                    _targetCell = GameController.Instance.GetClosestFriend(this).Location;

                    if (_targetCell == null)
                    {
                        _lastDecision = DecisionType.RandomMove;

                        // all friends are dead 
                        Move(
                            Location.GetDirection(
                                neighbors.Except(new[]
                                {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))}).ToArray()[
                                    Constants.Rnd.Next(neighbors.Length - 1)]));
                    }
                    else
                    {
                        _lastDecision = DecisionType.MoveToFriend;

                        // calculate scores for each direction
                        var directionScore = new int[4];

                        // iterate over directions
                        for (var i = 0; i < (int) Direction.Count; i++)
                        {
                            // get neighbor in that direction
                            var neighbor = Location.GetNeighbor((Direction) i);
                            // ignore neighbors that can't be moved to
                            if (neighbor == null || neighbor.IsWall)
                            {
                                directionScore[i] = Int32.MaxValue;
                                continue;
                            }

                            // calculate score: distance + rnd
                            directionScore[i] = neighbor.GetManhattenDistance(_targetCell)*
                                                neighbor.GetManhattenDistance(_targetCell) +
                                                Constants.Rnd.Next(Constants.Visibility);

                            // if we'd have to go backwards, double the score
                            if (((int) Orientation + 2%(int) Direction.Count) == i)
                                directionScore[i] *= 2;
                        }

                        // pick all directions with lowest costs
                        var bestDirections =
                            (from d in directionScore
                                where d == directionScore.Min()
                                select Array.IndexOf(directionScore, d)).ToArray();

                        // randomly chose one of the best directions
                        Move((Direction) bestDirections[Constants.Rnd.Next(bestDirections.Length)]);
                    }
                }
                else
                {
                    if (GameController.Instance == null)
                        return;

                    // find closest enemy
                    _targetCell = GameController.Instance.GetClosestOpponentCell(this);

                    if (_targetCell == null)
                    {
                        _lastDecision = DecisionType.RandomMove;

                        // no target found, move in random direction that is not backwards
                        Move(
                            Location.GetDirection(
                                neighbors.Except(new[] {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))})
                                    .ToArray()[
                                        Constants.Rnd.Next(neighbors.Length - 1)]));
                    }
                    else
                    {
                        _lastDecision = DecisionType.MoveToEnemy;

                        // calculate scores for each direction
                        var directionScore = new int[4];

                        // iterate over directions
                        for (var i = 0; i < (int) Direction.Count; i++)
                        {
                            // get neighbor in that direction
                            var neighbor = Location.GetNeighbor((Direction) i);
                            // ignore neighbors that can't be moved to
                            if (neighbor == null || neighbor.IsWall)
                            {
                                directionScore[i] = Int32.MaxValue;
                                continue;
                            }

                            // calculate score: distance * distance + rnd
                            directionScore[i] = neighbor.GetManhattenDistance(_targetCell)*
                                                neighbor.GetManhattenDistance(_targetCell) +
                                                Constants.Rnd.Next(Constants.Visibility);

                            // if we'd have to go backwards, double the score
                            if (((int) Orientation + 2%(int) Direction.Count) == i)
                                directionScore[i] *= 2;
                        }

                        // pick all directions with lowest costs
                        var bestDirections =
                            (from d in directionScore
                                where d == directionScore.Min()
                                select Array.IndexOf(directionScore, d)).ToArray();

                        // randomly chose one of the best directions
                        Move((Direction) bestDirections[Constants.Rnd.Next(bestDirections.Length)]);
                    }
                }
            }

            Thread.Sleep(Constants.AiMoveTimeout);
        }

        public static Tuple<Func<bool[], bool>[], bool[][], int[]> ParseTreeCreationData(DecisionData[] data)
        {
            var tests = new Func<bool[], bool>[]
            {
                d => d[0],
                d => d[1],
                d => d[2]
            };
            var attributes = new bool[data.Length][];
            var decisions = new int[data.Length];

            for (var i = 0; i < data.Length; i++)
            {
                attributes[i] = new[] { data[i].IsHealthy, data[i].HasAmmo, data[i].IsEnemyInRange };
                decisions[i] = (int)data[i].Decision;
            }

            return new Tuple<Func<bool[], bool>[], bool[][], int[]>(tests, attributes, decisions);
        }

        public static bool[] ParseTreeQueryData(DecisionData data)
        {
            return new[] { data.IsHealthy, data.HasAmmo, data.IsEnemyInRange };
        }

        #endregion
    }
}
