using System;
using System.Linq;
using System.Threading;
using Ai2dShooter.Common;
using Ai2dShooter.Map;

namespace Ai2dShooter.Model
{
    public class FsmPlayer : Player
    {
        #region Private Fields

        private enum State { SeekEnemy, Attack, SeekFriend, Dead, Count }

        private State _state;

        #endregion

        #region Constructor

        public FsmPlayer(Cell initialLocation, Teams team)
            : base(initialLocation, PlayerController.AiFsm, team)
        {
            LocationChanged += MovementDecision;
            HealthChanged += HealthDecision;
        }

        #endregion

        #region Main Methods

        public override void StartGame()
        {
            _state = State.SeekEnemy;
            
            new Thread(MovementDecision).Start();
        }

        public override void EnemySpotted()
        {
            _state = State.Attack;
            AbortMovement();

            // make decision?????
        }

        #endregion

        #region Event Handling

        private void HealthDecision()
        {
            switch (_state)
            {
                case State.SeekEnemy:
                    // attack as well!
                    break;
                case State.Attack:
                    // attack still?
                    break;
                case State.SeekFriend:
                    throw new NotImplementedException();
                case State.Dead:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MovementDecision()
        {
            while (IsMoving)
            {
                Thread.Sleep(Constants.MsPerCell / Constants.Framerate);
            }

            switch (_state)
            {
                case State.SeekEnemy:
                    // move in random direction, if possible don't go backwards
                    var neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();
                    if (neighbors.Length == 1)
                    {
                        // retreat
                        Move(Location.GetDirection(neighbors[0]));
                    }
                    else
                    {
                        // move in random direction that is not backwards
                        Move(Location.GetDirection(neighbors.Except(new[] {Location.GetNeighbor((Direction)(((int)Orientation + 2)%4))}).ToArray()[
                            Constants.Rnd.Next(neighbors.Length - 1)]));
                    }
                    break;
                case State.Attack:
                    // no movement when attacking
                    break;
                case State.SeekFriend:
                    throw new NotImplementedException();
                case State.Dead:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Thread.Sleep(Constants.AiMoveTimeout * 100);
        }

        #endregion
    }
}