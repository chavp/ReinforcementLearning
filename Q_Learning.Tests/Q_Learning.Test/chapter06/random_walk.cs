using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter06
{
    public class random_walk
    {
        ITestOutputHelper console;
        public random_walk(ITestOutputHelper console)
        {
            this.console = console;
        }

        static List<double> VALUES = null;
        static List<double> TRUE_VALUE = null;
        static readonly int ACTION_LEFT = 0;
        static readonly int ACTION_RIGHT = 1;

        Random Ran = new Random();
        (List<int>, List<double>) temporal_difference(
            List<double> values,
            double alpha = .1,
            bool batch = false)
        {
            int state = 3;
            var trajectoty = new List<int>
            {
                state
            };
            
            double returns = 0.0;
            while (true)
            {
                if(Ran.Next(0, 2) == ACTION_LEFT)
                {
                    state -= 1;
                }
                else
                {
                    state += 1;
                }
                trajectoty.Add(state);
                if(state == 6)
                {
                    returns = 1;
                    break;
                }
                else if (state == 0)
                {
                    returns = 0;
                    break;
                }
            }

            var rewards = new List<double>();
            if (!batch)
            {
                for (int i = 0; i < trajectoty.Count - 1; i++)
                {
                    var state_ = trajectoty[i];
                    values[state_] += alpha * (returns - values[state_]);
                    rewards.Add(returns);
                }
            }

            return (trajectoty, rewards);
        }

        (List<int>, List<double>) monte_carlo(
            List<double> values,
            double alpha = .1,
            bool batch = false)
        {
            int state = 3;
            var trajectoty = new List<int>
            {
                state
            };
            var rewards = new List<double>
            {
                0
            };
            while (true)
            {
                var old_state = state;
                if (Ran.Next(0, 2) == ACTION_LEFT)
                {
                    state -= 1;
                }
                else
                {
                    state += 1;
                }
                trajectoty.Add(state);
                var reward = 0.0;
                if(state == 6)
                {
                    rewards.Add(1);
                    break;
                }
                else if(state == 0)
                {
                    rewards.Add(0);
                    break;
                }
            }

            if (!batch)
            {
                foreach (var state_ in trajectoty)
                {

                }
            }

            return (trajectoty, rewards);
        }

        static random_walk()
        {
            VALUES = new List<double>()
            {
                0, .5, .5, .5, .5, .5, 1
            };

            TRUE_VALUE = new List<double>()
            {
                0, 0, 0, 0, 0, 0, 1
            };
            for (int i = 1; i < 6; i++)
            {
                TRUE_VALUE[i] = i / 6;
            }
        }

        [Fact]
        public void compute_state_value()
        {
            var episodes = new List<int> {
                0, 1, 10, 100
            };
            var current_values = VALUES.ToList();
            var current_values2 = VALUES.ToList();
            for (int i = 0; i < 101; i++)
            {
                (List<int> t1, List<double> r1) = temporal_difference(current_values);
                (List<int> t2, List<double> r2) = monte_carlo(current_values2);
            }
        }

        [Fact]
        public void figure_6_2()
        {

        }
    }
}
