using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ai2dShooter.Common;
using Ai2dShooter.Controller;
using Ai2dShooter.Map;
using Ai2dShooter.View;

namespace Ai2dShooter.Model
{
    /// <summary>
    /// An AI player controlled by a finite state machine (FSM).
    /// </summary>
    public sealed class FsmPlayer : Player
    {
        #region Private Fields

        private enum State { FindEnemy, AttackEnemy, Combat, FindFriend, Reload, Dead }

        private static readonly Color[] StateColors =
        {
            Color.Green, Color.Goldenrod, Color.Red, Color.Pink, Color.Purple, Color.FromArgb(Constants.DeadAlpha, Color.DeepSkyBlue)
        };

        private readonly Pen _targetPen = new Pen(Color.FromArgb(127, Color.LawnGreen), 4);

        /// <summary>
        /// Cell that the player is targetting.
        /// </summary>
        private Cell _targetCell;

        /// <summary>
        /// Current state.
        /// </summary>
        private State _state;

        /// <summary>
        /// Number of reloading steps remaining.
        /// </summary>
        private int _reloadSteps = -1;

        #endregion

        #region Constructor

        public FsmPlayer(Cell initialLocation, Teams team)
            : base(initialLocation, PlayerController.AiFsm, team)
        {
            // register to events
            LocationChanged += MakeDecision;
            HealthChanged += MakeDecision;
            Death += () => _state = State.Dead;
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
            graphics.DrawEllipse(new Pen(StateColors[(int)_state], penWidth), smallerBox);

            // if required, draw a line to the cell that is currently being targeted by the player
            if (_targetCell != null && IsAlive)
                graphics.DrawLine(_targetPen, box.X + box.Width/2, box.Y + box.Height/2, _targetCell.X * scaleFactor + box.Width / 2, _targetCell.Y*scaleFactor + box.Height / 2);
        }

        public override void StartGame()
        {
            // initial state: seek enemy
            _state = State.FindEnemy;

            // start own worker thread
            new Thread(MakeDecision).Start();
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

            // depending on health, look for friends or enemies or reload
            if (UsesKnife)
            {
                _state = State.Reload;
                _reloadSteps = Constants.ReloadSounds.Length - 1;
            }
            else
                _state = Health >= HealthyThreshold ? State.FindEnemy : State.FindFriend;

            //Console.WriteLine(this + " has killed an enemy and is now " + _state);

            // start movement
            MakeDecision();
        }

        private void MakeDecision()
        {
            if (IsMoving || !IsAlive || !PlayerExists)
                return;

            Cell[] neighbors;
            switch (_state)
            {
                case State.FindEnemy:
                case State.AttackEnemy:
                    // stuck?
                    neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();
                    if (neighbors.Length == 1)
                    {
                        _state = State.FindEnemy;
                        // backtrack
                        Move(Location.GetDirection(neighbors[0]));
                    }
                    else
                    {
                        // find closest enemy
                        if (GameController.Instance == null)
                            return;

                        _targetCell = GameController.Instance.GetClosestVisibleOpponentCell(this);
                        _state = _targetCell == null ? State.FindEnemy : State.AttackEnemy;

                        if (_targetCell == null)
                        {
                            // no target found, move to neighboring cell where influence is lowest. Dont go backwards.
                            Move(
                                Location.GetDirection(
                                    GameController.Instance.GetCellWithLowestInfluence(
                                        neighbors.Except(new[]
                                        {Location.GetNeighbor((Direction) (((int) Orientation + 2)%4))}).ToArray(), this)));
                        }
                        else
                        {
                            // target found

                            // calculate scores for each direction
                            var directionScore = new int[4];

                            // iterate over directions
                            for (var i = 0; i < (int)Direction.Count; i++)
                            {
                                // get neighbor in that direction
                                var neighbor = Location.GetNeighbor((Direction)i);
                                // ignore neighbors that can't be moved to
                                if (neighbor == null || neighbor.IsWall)
                                {
                                    directionScore[i] = int.MaxValue;
                                    continue;
                                }

                                // calculate score: distance * distance + rnd
                                directionScore[i] = neighbor.GetManhattenDistance(_targetCell)*
                                                    neighbor.GetManhattenDistance(_targetCell);

                                //// if we'd have to go backwards, double the score
                                //if (((int)Orientation + 2 % (int)Direction.Count) == i)
                                //    directionScore[i] *= 2;
                            }

                            // pick all directions with lowest costs
                            var bestDirections =
                                (from d in directionScore
                                 where d == directionScore.Min()
                                 select Array.IndexOf(directionScore, d)).ToArray();

                            // randomly chose one of the best directions
                            Move((Direction)bestDirections[Constants.Rnd.Next(bestDirections.Length)]);
                        }
                    }
                    break;
                case State.FindFriend:
                    // stuck?
                    neighbors = Location.Neighbors.Where(n => n != null && !n.IsWall).ToArray();
                    if (neighbors.Length == 1)
                    {
                        // backtrack
                        Move(Location.GetDirection(neighbors[0]));
                    }
                    else
                    {
                        if (GameController.Instance == null)
                            return;

                        // find closest friend
                        var friend = GameController.Instance.GetClosestFriend(this);
                        _targetCell = friend == null ? null : friend.Location;

                        if (_targetCell == null)
                        {
                            // all friends are dead or already follow me
                            // move in random direction
                            Move(
                                Location.GetDirection(
                                    neighbors.Except(new[] { Location.GetNeighbor((Direction)(((int)Orientation + 2) % 4)) }).ToArray()[
                                        Constants.Rnd.Next(neighbors.Length - 1)]));
                        }
                        else
                        {
                            // found friend to follow
                            FollowedPlayer = friend;

                            // calculate scores for each direction
                            var directionScore = new int[4];

                            // iterate over directions
                            for (var i = 0; i < (int)Direction.Count; i++)
                            {
                                // get neighbor in that direction
                                var neighbor = Location.GetNeighbor((Direction)i);
                                // ignore neighbors that can't be moved to
                                if (neighbor == null || neighbor.IsWall)
                                {
                                    directionScore[i] = int.MaxValue;
                                    continue;
                                }

                                // calculate score: distance * distance
                                directionScore[i] = neighbor.GetManhattenDistance(_targetCell)*
                                                    neighbor.GetManhattenDistance(_targetCell);

                                // if we'd have to go backwards, double the score
                                if (((int)Orientation + 2 % (int)Direction.Count) == i)
                                    directionScore[i] *= 2;
                            }

                            // pick all directions with lowest costs
                            var bestDirections =
                                (from d in directionScore
                                 where d == directionScore.Min()
                                 select Array.IndexOf(directionScore, d)).ToArray();

                            // randomly chose one of the best directions
                            Move((Direction)bestDirections[Constants.Rnd.Next(bestDirections.Length)]);
                        }
                    }
                    break;
                case State.Combat:
                    // keep fighting
                    break;
                case State.Reload:
                    if (_reloadSteps < 0)
                    {
                        Ammo = MaxAmmo;
                        MakeDecision();
                        return;
                    }
                    lock (Constants.MovementLock)
                    {
                        if (MainForm.Instance.PlaySoundEffects)
                            MainForm.Instance.Invoke((MethodInvoker) (() => Constants.ReloadSounds[_reloadSteps--].Play()));

                        if (_reloadSteps <= -1)
                        {
                            _state = Health >= HealthyThreshold ? State.FindEnemy : State.FindFriend;
                            Ammo = MaxAmmo;
                        }
                        new Thread(() =>
                        {
                            Thread.Sleep(Constants.ReloadTimeout);
                            MakeDecision();
                        }).Start();
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Thread.Sleep(Constants.AiMoveTimeout);
        }

        #endregion
    }
}
