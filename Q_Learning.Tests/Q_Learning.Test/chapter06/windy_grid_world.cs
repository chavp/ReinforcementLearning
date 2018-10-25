using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter06
{
    public class windy_grid_world
    {
        ITestOutputHelper console;
        public windy_grid_world(ITestOutputHelper console)
        {
            this.console = console;
        }

        static readonly int WORLD_HEIGHT = 7;
        static readonly int WORLD_WIDTH = 10;
        static readonly int[] WIND = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 1, 0 };

        static readonly int ACTION_UP = 0;
        static readonly int ACTION_DOWN = 1;
        static readonly int ACTION_LEFT = 2;
        static readonly int ACTION_RIGHT = 3;
        static readonly int[] ACTIONS = new int[] { ACTION_UP, ACTION_DOWN, ACTION_LEFT, ACTION_RIGHT };

        static readonly double EPSILON = 0.1;
        static readonly double ALPHA = 0.5;
        static readonly double REWARD = -1;

        static readonly (int, int) START = (3, 0);
        static readonly (int, int) GOAL = (3, 7);
        
        static (int, int) Step((int, int) state, int action)
        {
            (int i, int j) = state;
            if (action == ACTION_UP)
                return (Math.Max(i - 1 - WIND[j], 0), j);
            else if (action == ACTION_DOWN)
                return (Math.Max(Math.Min(i + 1 - WIND[j], WORLD_HEIGHT - 1), 0), j);
            else if (action == ACTION_LEFT)
                return (Math.Max(i - WIND[j], 0), Math.Max(j - 1, 0));
            else if (action == ACTION_RIGHT)
                return (Math.Max(i - WIND[j], 0), Math.Min(j + 1, WORLD_WIDTH - 1));
            else
                throw new ArgumentOutOfRangeException("Not implement Step");
        }

        Random Ran = new Random();
        public int ChooseAction((int, int) state, Dictionary<(int, int, int), double> q_value)
        {
            if (Ran.NextDouble() < EPSILON)
            {
                return ACTIONS[Ran.Next(0, ACTIONS.Length)];
            }
            else
            {
                var values_ = q_value.Where(x => x.Key.Item1 == state.Item1
                && x.Key.Item2 == state.Item2).ToList();

                var maxActs = values_.Where(x => x.Value == values_.Max(y => y.Value)).ToList();
                return maxActs[Ran.Next(0, maxActs.Count)].Key.Item3;
            }
        }

        public int Episode(Dictionary<(int, int, int), double> q_value)
        {
            int time = 0;

            var state = START;

            int action = ChooseAction(state, q_value);

            while (state.Item1 != GOAL.Item1 || state.Item2 != GOAL.Item2)
            {
                var next_state = Step(state, action);
                var next_action = ChooseAction(next_state, q_value);

                q_value[(state.Item1, state.Item2, action)]
                    += ALPHA * (REWARD + q_value[(next_state.Item1, next_state.Item2, next_action)]
                    - q_value[(state.Item1, state.Item2, action)]);

                state = next_state;
                action = next_action;
                ++time;
            }

            return time;
        }

        [Fact]
        public void figure_6_3()
        {
            var q_value = new Dictionary<(int, int, int), double>();
            for (int i = 0; i < WORLD_HEIGHT; i++)
            {
                for (int j = 0; j < WORLD_WIDTH; j++)
                {
                    for (int k = 0; k < ACTIONS.Length; k++)
                    {
                        var state_act = (i, j, k);
                        q_value.Add(state_act, 0);
                    }
                }
            }

            int episode_limit = 500;
            int ep = 0;
            var steps = new List<int>();
            while (ep < episode_limit)
            {
                steps.Add(Episode(q_value));
                ++ep;
            }
        }
    }
}
