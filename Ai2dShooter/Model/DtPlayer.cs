using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Controller;
using Ai2dShooter.Map;

namespace Ai2dShooter.Model
{
    public class DtPlayer : Player
    {
        #region Private Fields

        private enum Decision { MoveToEnemy, MoveToFriend, RandomMove, Backtrack, Count };

        private static readonly Color[] DecisionColors =
        {
            Color.Goldenrod, Color.Pink, Color.Green, Color.Gray
        };

        private readonly Pen _opponentPen = new Pen(Color.Orange, 2);

        private Cell _targetCell;

        private Decision _lastDecision = Decision.Count;

        private bool _inCombat;

        #endregion

        #region Constructor

        public DtPlayer(Cell initialLocation, Teams team)
            : base(initialLocation, PlayerController.AiFsm, team)
        {
            LocationChanged += MakeDecision;
            HealthChanged += MakeDecision;
            Death += () => _lastDecision = Decision.Count;
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
            if (_lastDecision != Decision.Count)
            {
                var edges = new[]
                {
                    new PointF(smallerBox.Left + (float)smallerBox.Width/3, smallerBox.Top),
                    new PointF(smallerBox.Left + (float)smallerBox.Width*2/3, smallerBox.Top),
                    new PointF(smallerBox.Right, smallerBox.Top + (float)smallerBox.Height/3),
                    new PointF(smallerBox.Right, smallerBox.Top + (float)smallerBox.Height*2/3),
                    new PointF(smallerBox.Left + (float)smallerBox.Width*2/3, smallerBox.Bottom),
                    new PointF(smallerBox.Left + (float)smallerBox.Width/3, smallerBox.Bottom),
                    new PointF(smallerBox.Left, smallerBox.Top + (float)smallerBox.Height*2/3),
                    new PointF(smallerBox.Left, smallerBox.Top + (float)smallerBox.Height/3),
                };
                graphics.DrawPolygon(new Pen(DecisionColors[(int)_lastDecision], penWidth), edges);
            }

            // if required, draw a line to the cell that is currently being targeted by the player
            if (_targetCell != null && IsAlive)
                graphics.DrawLine(_opponentPen, box.X + box.Width/2, box.Y + box.Height/2,
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

            _lastDecision = Decision.Count;

            // stuck?
            if (neighbors.Length == 1)
            {
                _lastDecision = Decision.Backtrack;

                // backtrack
                Move(Location.GetDirection(neighbors[0]));
            }
            else
            {
                // healthy?
                if (Health < HealthyThreshold)
                {
                    // find closest friend
                    _targetCell = GameController.Instance.GetClosestFriendCell(this);

                    if (_targetCell == null)
                    {
                        _lastDecision = Decision.RandomMove;

                        // all friends are dead 
                        Move(
                            Location.GetDirection(
                                neighbors.Except(new[]
                                {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))}).ToArray()[
                                    Constants.Rnd.Next(neighbors.Length - 1)]));
                    }
                    else
                    {
                        _lastDecision = Decision.MoveToFriend;

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
                                directionScore[i] = int.MaxValue;
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
                    // find closest enemy
                    _targetCell = GameController.Instance.GetClosestOpponentCell(this);

                    if (_targetCell == null)
                    {
                        _lastDecision = Decision.RandomMove;

                        // no target found, move in random direction that is not backwards
                        Move(
                            Location.GetDirection(
                                neighbors.Except(new[] {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))})
                                    .ToArray()[
                                        Constants.Rnd.Next(neighbors.Length - 1)]));
                    }
                    else
                    {
                        _lastDecision = Decision.MoveToEnemy;

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
                                directionScore[i] = int.MaxValue;
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

        #endregion

    }
}
