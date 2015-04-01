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
            graphics.DrawEllipse(new Pen(StateColors[_state], 3), box);

            if (_targetCell != null && IsAlive)
            {
                graphics.DrawLine(_opponentPen, box.X + box.Width/2, box.Y + box.Height/2, _targetCell.X * scaleFactor + box.Width / 2, _targetCell.Y*scaleFactor + box.Height / 2);
            }
        }

        public override void StartGame()
        {
            _state = State.SeekEnemy;

            new Thread(MovementDecision).Start();
        }

        public override void EnemySpotted()
        {
            _state = State.Combat;
            AbortMovement();
        }

        public override void SpottedByEnemy()
        {
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

        private void HealthDecision()
        {
            //Console.WriteLine("HEALTH STATE: " + _state);
            switch (_state)
            {
                case State.SeekEnemy:
                    // attack as well
                    _state = State.Combat;
                    break;
                case State.Combat:
                    // keep attacking
                    break;
                case State.SeekFriend:
                    throw new NotImplementedException();
                case State.Dead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MovementDecision()
        {
            if (IsMoving || !IsAlive)
                return;

            //Console.WriteLine("MOVEMENT STATE: " + _state);
            switch (_state)
            {
                case State.SeekEnemy:
                case State.SeekFriend:
                    // stuck?
                    var neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();
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

                                // calculate score: distance + rnd
                                directionScore[i] = neighbor.GetManhattenDistance(_targetCell) +
                                                    Constants.Rnd.Next(2*Constants.Visibility);

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
                case State.Combat:
                    // no movement when attacking
                    break;
                    // TODO
                    Console.WriteLine("I DONT KNOW HOW TO SEEK FRIENDS YET!");
                    break;
                case State.Dead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Thread.Sleep(Constants.AiMoveTimeout);
        }

        #endregion
    }
}
