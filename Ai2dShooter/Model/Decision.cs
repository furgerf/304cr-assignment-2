using System;

namespace Ai2dShooter.Model
{
    public class Decision
    {
        #region Public Fields

        protected Decision Positive { get; private set; }

        protected Decision Negative { get; private set; }

        protected Func<bool[], bool> Test { get; private set; }

        public enum DecisionType { MoveToEnemy, MoveToFriend, RandomMove, Backtrack, Reload, Count }

        private static readonly Decision[] Decisions =
        {
            new Decision(DecisionType.MoveToEnemy), new Decision(DecisionType.MoveToFriend),
            new Decision(DecisionType.RandomMove), new Decision(DecisionType.Backtrack),
            new Decision(DecisionType.Reload)
        };

        public DecisionType Type { get; private set; }

        #endregion

        #region Constructor

        // constructor for leaves
        public Decision(DecisionType type)
        {
            Type = type;
        }

        // constructor for internal nodes
        protected Decision(Decision positive, Decision negative, Func<bool[], bool> test)
        {
            Positive = positive;
            Negative = negative;
            Test = test;
            Type = DecisionType.Count;
        }

        #endregion

        #region Main Methods

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
                attributes[i] = new[]{data[i].IsHealthy, data[i].HasAmmo, data[i].IsEnemyInRange};
                decisions[i] = (int)data[i].Decision;
            }

            return new Tuple<Func<bool[], bool>[], bool[][], int[]>(tests, attributes, decisions);
        }

        public static bool[] ParseTreeQueryData(DecisionData data)
        {
            return new[] {data.IsHealthy, data.HasAmmo, data.IsEnemyInRange};
        }

        public virtual Decision Evaluate(bool[] data)
        {
            if (Type == DecisionType.Count)
                throw new Exception("Internal node should be overridden");

            return Decisions[(int) Type];
        }

        #endregion
    }
}
