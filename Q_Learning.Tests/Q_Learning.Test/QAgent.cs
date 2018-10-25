using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Q_Learning.Test
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Q-learning
    /// </summary>
    public class QAgent
    {
        static double Epsilon = 0.8; // greedy police (1 greedy, 0 non greedy)
        readonly static double Alpha = 0.1; // Learning rate
        readonly static double Gamma = 0.9; // Discount factor

        static int NumActions = 0;
        static int NumStates = 0;

        static Random Random;
        static Dictionary<(int, int), double> Q;

        public QAgent(int states, int actions)
        {
            NumActions = actions;
            NumStates = states;
            Random = new Random(DateTime.Now.Second);

            Q = new Dictionary<(int, int), double>();

            for (int i = 0; i < NumStates; i++)
            {
                for (int j = 0; j < NumActions; j++)
                {
                    Q.Add((i, j), 0.0);
                }
            }
        }


        public Func<int, (int, string)> ChooseAction = (state) =>
        {
            var state_actions = (from x in Q
                                 where x.Key.Item1 == state
                                 select x);

            // Act non-greedy or state-action have no value
            if (Random.NextDouble() > Epsilon
                || state_actions.Sum(x => x.Value) == 0)
            {
                return (Random.Next(0, NumActions), "non-greedy");
            }
            else
            {
                // Act greedy
                return (GetRandomActionIsMaxRewardByState(state), "greedy");
            }
        };

        public Action<int, int, int, double, bool> Learn =
            (state, action, nextState, reward, done) =>
        {
            var learnValue = reward + Gamma * GetMaxRewardByState(nextState);
            var oldValue = Q[(state, action)];
            if (done)
            {
                learnValue = reward;
            }

            // New estimate
            Q[(state, action)] = (1 - Alpha) * oldValue + Alpha * learnValue;
        };

        // argmax a f
        public static int GetRandomActionIsMaxRewardByState(int state)
        {
            var actions_by_state = (from x in Q
                                 where x.Key.Item1 == state
                                 select x);

            var actionsHasMaxReward = (from x in actions_by_state
                                    where x.Value == (actions_by_state.Max(y => y.Value))
                             select x.Key.Item2).ToList();

            return actionsHasMaxReward[Random.Next(0, actionsHasMaxReward.Count)];
        }

        public void ReduceExploration(double i)
        {
            Epsilon = Epsilon / (1 + i);
        }

        public static double GetMaxRewardByState(int state)
        {
            var action_is_max_reward = GetRandomActionIsMaxRewardByState(state);
            return Q[(state, action_is_max_reward)];
        }

        public Dictionary<(int, int), double> QTable
        {
            get
            {
                return Q;
            }
        }

        public void RenderQ(ITestOutputHelper console = null)
        {
            for (int state = 0; state < NumStates; state++)
            {
                var rows = from x in Q
                           where x.Key.Item1 == state
                           select x.Value;

                var msg = $"({state}) = [ {string.Join(",", rows)} ]";
                if (console != null)
                    console.WriteLine(msg);
                else
                    System.Console.WriteLine(msg);
                
            }
        }
    }
}
