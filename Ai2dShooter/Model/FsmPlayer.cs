using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Controller;
using Ai2dShooter.Map;

namespace Ai2dShooter.Model
{
    public class FsmPlayer : Player
    {
        #region Private Fields

        private enum State { SeekEnemy, Combat, SeekFriend, Dead }

        private static readonly Dictionary<State, Color> StateColors = new Dictionary<State, Color>
        {
            {State.SeekEnemy, Color.Green},
            {State.Combat, Color.Red},
            {State.SeekFriend, Color.Pink},
            {State.Dead, Color.FromArgb(Constants.DeadAlpha, Color.DeepSkyBlue)}
        };

        private readonly Pen _opponentPen = new Pen(Color.Orange, 2);

        private Cell _targetCell;

        private State _state;

        #endregion

        #region Constructor

        public FsmPlayer(Cell initialLocation, Teams team)
            : base(initialLocation, PlayerController.AiFsm, team)
        {
            LocationChanged += MovementDecision;
            HealthChanged += HealthDecision;
            Death += () => _state = State.Dead;
        }

        #endregion

        #region Main Methods

        protected override void DrawPlayerImplementation(Graphics graphics, int scaleFactor, Rectangle box)
        {
            // make the box a bit smaller (so the circle doesn't exceed the box)
            var smallerBox = box;
            smallerBox.Inflate((int)-_opponentPen.Width, (int)-_opponentPen.Width);

            // draw circle in the color belonging to the current state
            graphics.DrawEllipse(new Pen(StateColors[_state], 3), smallerBox);

            // if required, draw a line to the cell that is currently being targeted by the player
            if (_targetCell != null && IsAlive)
                graphics.DrawLine(_opponentPen, box.X + box.Width/2, box.Y + box.Height/2, _targetCell.X * scaleFactor + box.Width / 2, _targetCell.Y*scaleFactor + box.Height / 2);
        }

        public override void StartGame()
        {
            // initial state: seek enemy
            _state = State.SeekEnemy;

            // start own worker thread
            new Thread(MovementDecision).Start();
        }

        public override void EnemySpotted()
        {
            // stop moving and start fighting
            _state = State.Combat;
            AbortMovement();
        }

        public override void SpottedByEnemy()
        {
            // stop moving and start fighting
            _state = State.Combat;
            AbortMovement();
        }

        public override void KilledEnemy()
        {
            base.KilledEnemy();

            // depending on health, look for friends or enemies
            _state = Health >= HealthyThreshold ? State.SeekEnemy : State.SeekFriend;

            // start movement
            MovementDecision();
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Should only lose life while in combat, and then there's no need to do anything.
        /// </summary>
        private void HealthDecision()
        {
            //Console.WriteLine("HEALTH STATE: " + _state);
            switch (_state)
            {
                case State.Combat:
                    // keep attacking
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Make a decision in which direction to move.
        /// </summary>
        private void MovementDecision()
        {
            if (IsMoving || !IsAlive)
                return;

            //Console.WriteLine("MOVEMENT STATE: " + _state);
            Cell[] neighbors;
            switch (_state)
            {
                case State.SeekEnemy:
                    // stuck?
                    neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();
                    if (neighbors.Length == 1)
                    {
                        // backtrack
                        Move(Location.GetDirection(neighbors[0]));
                    }
                    else
                    {
                        // find closest enemy
                        _targetCell = GameController.Instance.GetClosestOpponentCell(this);

                        if (_targetCell == null)
                        {
                            // no target found, move in random direction that is not backwards
                            Move(
                                Location.GetDirection(
                                    neighbors.Except(new[]
                                    {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))}).ToArray()[
                                        Constants.Rnd.Next(neighbors.Length - 1)]));
                        }
                        else
                        {
                            // calculate scores for each direction
                            var directionScore = new int[4];

                            // iterate over directions
                            for (var i = 0; i < (int)Direction.Count; i++)
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
                                directionScore[i] = neighbor.GetManhattenDistance(_targetCell) * neighbor.GetManhattenDistance(_targetCell) +
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
                    break;
                case State.SeekFriend:
                    // stuck?
                    neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();
                    if (neighbors.Length == 1)
                    {
                        // backtrack
                        Move(Location.GetDirection(neighbors[0]));
                    }
                    else
                    {
                        // TODO: Maybe also take enemies into account (flee)
                        // find closest friend
                        _targetCell = GameController.Instance.GetClosestFriendCell(this);

                        if (_targetCell == null)
                        {
                            // all friends are dead 
                            Move(
                                Location.GetDirection(
                                    neighbors.Except(new[]
                                    {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))}).ToArray()[
                                        Constants.Rnd.Next(neighbors.Length - 1)]));
                        }
                        else
                        {
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Thread.Sleep(Constants.AiMoveTimeout);
        }

        #endregion
    }
}
