using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ai2dShooter.Common;

namespace Ai2dShooter.Model
{
    public class DecisionTree : Decision
    {
        #region Public Fields

        #endregion

        #region Constructor

        private DecisionTree(Decision positive, Decision negative, Func<bool[], bool> test)
            :base(positive, negative, test)
        {

        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Creates a decision tree.
        /// </summary>
        /// <param name="tests">Array of tests, one item for one attribute. THIS IS NECESSARY because indices change</param>
        /// <param name="data">Matrix of data, the outer array being the rows, the inner arrays the columns</param>
        /// <param name="decisions">Decisions that are made with those data, one entry for one row</param>
        /// <returns>New decision tree</returns>
        public static DecisionTree CreateTree(Func<bool[], bool>[] tests, bool[][] data, int[] decisions)
        {
            if (
                tests.Length != data[0].Length ||
                decisions.Length != data.Length)
                throw new ArgumentException("Dimension mismatch");

            // are we doing a node that has two leaves?
            if (data.Length == 2 && data[0].Length == 1)
            {
                return new DecisionTree(new Decision((DecisionType) (data[0][0] ? decisions[0] : decisions[1])),
                    new Decision((DecisionType) (data[0][0] ? decisions[1] : decisions[0])), tests[0]);
            }

            // calculate entropy for each attribute
            var entropy = new double[data[0].Length*2];
            // TODO

            // calculate information gain for each attribute
            var gain = new double[data[0].Length];
            for (var i = 0; i < gain.Length; i++)
                gain[i] = i;
                //gain[i] = Constants.Rnd.NextDouble();
            // TODO

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

            return new DecisionTree(
                CreateTree(newTests, positiveData.ToArray(), positiveDecisions.ToArray()),
                CreateTree(newTests, negativeData.ToArray(), negativeDecisions.ToArray()),
                tests[bestGainAttribute]);
        }

        public override Decision Evaluate(bool[] data)
        {
            return Test(data) ? Positive.Evaluate(data) : Negative.Evaluate(data);
        }

        #endregion
    }
}
