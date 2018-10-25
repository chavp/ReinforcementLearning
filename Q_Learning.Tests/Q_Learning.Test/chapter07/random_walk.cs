using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter07
{
    public class random_walk
    {
        ITestOutputHelper console;
        public random_walk(ITestOutputHelper console)
        {
            this.console = console;
        }

        static readonly int N_STATES = 19;
        static readonly int GAMMA = 1;
        static readonly List<int> STATES = Enumerable.Range(1, N_STATES + 1).ToList();
        static readonly int START_STATE = 10;
        static readonly List<int> END_STATES = new List<int> { 0, N_STATES + 1 };
        static readonly List<double> TRUE_VALUE = null;

        static random_walk()
        {
            TRUE_VALUE = new List<double>();
            for (int i = -20; i < 22; i+=2)
            {
                TRUE_VALUE.Add(i / 20.0);
            }
            TRUE_VALUE[0] = 0;
            TRUE_VALUE[TRUE_VALUE.Count - 1] = 0;
        }

        Random Ran = new Random();
        void temporal_difference(List<double> value, int n, double alpha)
        {
            var state = START_STATE;
            var states = new List<int> { state };
            var rewards = new List<double> { 0 };

            var time = 0;
            var T = int.MaxValue;
            var next_state = 0;
            var reward = 0.0;
            var update_time = 0;
            var returns = 0.0;
            while (true)
            {
                time += 1;
                if(time < T)
                {
                    if(Ran.Next(0, 2) == 1)
                    {
                        next_state = state + 1;
                    }
                    else
                    {
                        next_state = state - 1;
                    }

                    if(next_state == 0)
                    {
                        reward = -1;
                    }
                    else if (next_state == 20)
                    {
                        reward = 1;
                    }
                    else
                    {
                        reward = 0;
                    }

                    states.Add(next_state);
                    rewards.Add(reward);

                    if(END_STATES.Contains(next_state))
                    {
                        T = time;
                    }
                }

                update_time = time - n;
                if(update_time >= 0)
                {
                    returns = 0.0;
                    for (int t = update_time + 1; t < Math.Min(T, update_time + n) + 1; t++)
                    {
                        returns += Math.Pow(GAMMA, t - update_time - 1) * rewards[t];
                    }
                    if(update_time + n <= T)
                        returns += Math.Pow(GAMMA, n) * value[states[(update_time + n)]];
                    var state_to_update = states[update_time];
                    if (!END_STATES.Contains(state_to_update))
                    {
                        value[state_to_update] += alpha * (returns - value[state_to_update]);
                    }
                }
                if (update_time == T - 1)
                    break;
                state = next_state;
            }
        }

        [Fact]
        public void figure7_2()
        {
            var steps = Enumerable.Range(0, 10).Select( x => Math.Pow(2, x));
            var alphas = new List<double>();
            for (double i = 0; i < 1.1; i+=0.1)
            {
                alphas.Add(i);
            }
            var episodes = 10;
            var runs = 100;

        }
    }
}
