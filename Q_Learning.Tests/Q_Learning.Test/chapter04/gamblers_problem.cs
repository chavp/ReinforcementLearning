using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter04
{
    public class gamblers_problem
    {
        ITestOutputHelper console;
        public gamblers_problem(ITestOutputHelper console)
        {
            this.console = console;
        }

        readonly static int Goal = 100;
        readonly static List<int> States;
        readonly static double HeadProb = 0.4;

        static gamblers_problem()
        {
            States = Enumerable.Range(0, Goal + 1).ToList();
        }

        [Fact]
        public void figure_4_3()
        {
            var stateValue = new List<double>();
            for (int i = 0; i < Goal + 1; i++)
            {
                stateValue.Add(0.0);
            }
            stateValue[Goal] = 1;

            while (true)
            {
                var delta = 0.0;
                for (int state = 1; state < Goal; state++)
                {
                    var actions = Enumerable.Range(0, Math.Min(state, Goal - state) + 1).ToList();
                    var actionReturns = new List<double>();
                    foreach (var action in actions)
                    {
                        actionReturns.Add(HeadProb * stateValue[state + action] + (1 - HeadProb) * stateValue[state - action]);
                    }
                    var new_value = actionReturns.Max();
                    delta += Math.Abs(stateValue[state] - new_value);
                    stateValue[state] = new_value;
                }
                if(delta < 0.000000009)
                {
                    break;
                }
            }

            var policy = new List<double>();
            for (int i = 0; i < Goal + 1; i++)
            {
                policy.Add(0.0);
            }
            for (int state = 1; state < Goal; state++)
            {
                var actions = Enumerable.Range(0, Math.Min(state, Goal - state) + 1).ToList();
                var actionReturns = new List<double>();
                foreach (var action in actions)
                {
                    actionReturns.Add(HeadProb * stateValue[state + action] + (1 - HeadProb) * stateValue[state - action]);
                }
                // policy[state] = actions[np.argmax(np.round(action_returns[1:], 5)) + 1]
                var skipOneTakeFive = actionReturns.Skip(1).ToList();
                var argmaxIndex = skipOneTakeFive.IndexOf(skipOneTakeFive.Max());
                policy[state] = actions[argmaxIndex + 1];
            }

            for (int i = 0; i < policy.Count; i++)
            {
                console.WriteLine($"state: {i}, action: {policy[i]}");
            }
        }
    }
}
