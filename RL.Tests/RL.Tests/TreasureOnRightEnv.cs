using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using System.Threading.Tasks;

namespace RL.Tests
{
    public class TreasureOnRightEnv
    {
        int N_STATES = 6; // the length of the 1 dimensional world
        string[] ACTIONS = new string[] { "left", "right" }; // available actions
        double EPSILON = 0.9; // greedy police (0.9, 0.107438016528926) (0.1, 0.0379008746355685)
        double ALPHA = 0.2; // learning rate
        double GAMMA = 0.9; // discount factor

        Dictionary<(int, string), double> Q = new Dictionary<(int, string), double>();
        Random random = new Random(DateTime.Now.Second);

        public TreasureOnRightEnv()
        {
            for (int i = 0; i < N_STATES; i++)
            {
                foreach (var act in ACTIONS)
                {
                    var key = (i, act);
                    Q.Add(key, 0.0);
                }
            }
        }

        public string ChooseAction(int state)
        {
            Func<int, string> policy = (s) =>
            {
                var act = string.Empty;
                var s_actions = (from x in Q
                                     where x.Key.Item1 == state
                                     select x);
                //var state_actions = df.Where((data) => data.Key.Item1 == state);
                if (random.NextDouble() > EPSILON
                    || s_actions.Sum(x => x.Value) == 0)
                {
                    act = ACTIONS[random.Next(0, ACTIONS.Length)];
                }
                else
                {
                    // greedy policy
                    var max_state_actions = (from x in s_actions
                                             where x.Value == s_actions.Max(y => y.Value)
                                             select x).ToList();
                    var max_state_action = max_state_actions[random.Next(0, max_state_actions.Count())];
                    act = max_state_action.Key.Item2;
                }
                return act;
            };

            return policy(state);
        }

        public (int, double, bool) Step(int state, string action)
        {
            int nextState = state;
            bool done = false;
            if (action == "right")
            {
                if (state == N_STATES - 2)
                {
                    done = true;
                }
                else
                {
                    nextState = state + 1;
                }
            }
            else
            {
                if (state > 0)
                    nextState = state - 1;
            }

            double reward = Reward(state, action, nextState);

            return (nextState, reward, done);
        }

        public double Reward(int state, string action, int nextState)
        {
            double reward = 0;
            if (action == "right")
            {
                if(state == nextState) reward = 1;
            }

            return reward;
        }

        public void Learn(int state, string action, int nextState, double reward, bool done)
        {
            // https://en.wikipedia.org/wiki/State%E2%80%93action%E2%80%93reward%E2%80%93state%E2%80%93action
            var qPredict = Q[(state, action)];
            var qTarget = 0.0;
            if (!done)
            {
                var maxReward = (from x in Q
                                 where x.Key.Item1 == nextState
                                 select x.Value).Max();
                qTarget = reward + GAMMA * maxReward;
            }
            else
            {
                qTarget = reward;
            }

            Q[(state, action)] += ALPHA * (qTarget - qPredict);
        }

        public void Render(int state, double episode, int stepCounter, bool done, ITestOutputHelper output)
        {
            if (done)
            {
                output.WriteLine($"Episode {episode + 1}: total_steps = {stepCounter}");
            }
            else
            {
                var envList = Enumerable
                    .Range(0, N_STATES - 2)
                    .Select( x => "-").ToList();
                envList.Add("-");
                envList.Add("T");
                envList[state] = "o";
                output.WriteLine($"{string.Join("", envList)}");
            }
        }
    }
}
