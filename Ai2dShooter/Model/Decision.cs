using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ai2dShooter.Model
{
    /// <summary>
    /// Represents a node of a decision tree.
    /// </summary>
    public sealed class Decision
    {
        #region Public Fields

        /// <summary>
        /// Reference to the subtree that should be evaluated if the current test is positive.
        /// </summary>
        private Decision Positive { get; set; }

        /// <summary>
        /// Reference to the subtree that should be evaluated if the current test is negative.
        /// </summary>
        private Decision Negative { get; set; }

        /// <summary>
        /// Test (decision) on the current node.
        /// </summary>
        private Func<bool[], bool> Test { get; set; }

        /// <summary>
        /// Value of a leaf node (decision that is made).
        /// </summary>
        public int Type { get; private set; }

        #endregion

        #region Constructor

        // constructor for leaves
        private Decision(int type)
        {
            Type = type;
        }

        // constructor for internal nodes
        private Decision(Decision positive, Decision negative, Func<bool[], bool> test)
        {
            Positive = positive;
            Negative = negative;
            Test = test;
            Type = -1;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Creates a decision tree.
        /// </summary>
        /// <param name="tests">Array of tests, one item for one attribute. THIS IS NECESSARY because indices change!</param>
        /// <param name="data">Matrix of data, the outer array being the rows, the inner arrays the columns</param>
        /// <param name="decisions">Decisions that are made with those data, one entry for one row</param>
        /// <returns>New decision tree</returns>
        public static Decision CreateTree(Func<bool[], bool>[] tests, bool[][] data, int[] decisions)
        {
            var decisionCount = decisions.Max() + 1;

            // parameter checks
            if (tests.Length != data[0].Length || decisions.Length != data.Length)
                throw new ArgumentException("Dimension mismatch");

            // are we doing a node that has two leaves?
            if (data.Length == 2 && data[0].Length == 1)
            {
                return new Decision(new Decision(data[0][0] ? decisions[0] : decisions[1]),
                    new Decision(data[0][0] ? decisions[1] : decisions[0]), tests[0]);
            }

            // calculate entropy for each attribute
            var entropy = new double[data[0].Length * 2];
            var decisionCounts = new int[entropy.Length][];
            for (var i = 0; i < decisionCounts.Length; i++)
                decisionCounts[i] = new int[decisionCount];

            // count decisions
            for (var i = 0; i < data[0].Length; i++)    // iterate over attributes
            {
                for (var j = 0; j < data.Length; j++)   // iterate over rows
                {
                    // count decisions made when the current attribute is true/false
                    var offset = data[j][i] ? 0 : 1;
                    decisionCounts[2 * i + offset][decisions[j]]++;
                }
            }

            // actually calculate entropy now
            for (var i = 0; i < entropy.Length; i++)
            {
                var totalCount = (double)decisionCounts[i].Sum();
                foreach (var dc in decisionCounts[i])
                    entropy[i] += dc == 0 ? 0 : -dc / totalCount * Math.Log(dc / totalCount, 2);
            }

            // calculate information gain for each attribute
            var gain = new double[data[0].Length];
            for (var i = 0; i < gain.Length; i++)
                gain[i] = entropy[2 * i] + entropy[2 * i + 1];

            // find attribute with best gain
            var bestGainAttribute = Array.IndexOf(gain, gain.Max());

            // create new tests array
            var newTests = tests.Where((t, i) => i != bestGainAttribute).ToArray();

            // remove all entries that are redundant when disregarding the attribute
            var positiveData = new List<bool[]>();
            var positiveDecisions = new List<int>();
            var negativeData = new List<bool[]>();
            var negativeDecisions = new List<int>();
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i][bestGainAttribute])
                {
                    positiveData.Add(data[i].Where((d, j) => j != bestGainAttribute).ToArray());
                    positiveDecisions.Add(decisions[i]);
                }
                else
                {
                    negativeData.Add(data[i].Where((d, j) => j != bestGainAttribute).ToArray());
                    negativeDecisions.Add(decisions[i]);
                }
            }

            return new Decision(
                CreateTree(newTests, positiveData.ToArray(), positiveDecisions.ToArray()),
                CreateTree(newTests, negativeData.ToArray(), negativeDecisions.ToArray()),
                tests[bestGainAttribute]);
        }

        public Decision Evaluate(bool[] data)
        {
            if (Type == -1)
                return Test(data) ? Positive.Evaluate(data) : Negative.Evaluate(data);

            return new Decision(Type);
        }

        public override string ToString()
        {
            return Type == -1 ? "Inner Node" : "Decision " + Type.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
