using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using Xunit;

namespace Q_Learning.Test.chapter06
{
    public class cliff_walking
    {
        ITestOutputHelper console;
        public cliff_walking(ITestOutputHelper console)
        {
            this.console = console;
        }

        static readonly int WORLD_HEIGHT = 4;
        static readonly int WORLD_WIDTH = 12;

        // probability for exploration
        static readonly double EPSILON = 0.1;

        // step size
        static readonly double ALPHA = 0.5;

        // gamma for Q-Learning and Expected Sarsa
        static readonly double GAMMA = 1;

        static readonly int ACTION_UP = 0;
        static readonly int ACTION_DOWN = 1;
        static readonly int ACTION_LEFT = 2;
        static readonly int ACTION_RIGHT = 3;
        static readonly int[] ACTIONS = new int[] { ACTION_UP, ACTION_DOWN, ACTION_LEFT, ACTION_RIGHT };

        static readonly (int, int) START = (3, 0);
        static readonly (int, int) GOAL = (3, 11);

        public ((int, int), double) Step((int, int) state, int action)
        {
            (int i, int j) = state;
            (int, int) next_state;
            double reward = -1;
            if (action == ACTION_UP)
                next_state = (Math.Max(i - 1, 0), j);
            else if(action == ACTION_LEFT)
                next_state = (i, Math.Max(j - 1, 0));
            else if (action == ACTION_RIGHT)
                next_state = (i, Math.Min(j + 1, WORLD_WIDTH - 1));
            else if (action == ACTION_DOWN)
                next_state = (Math.Min(i + 1, WORLD_HEIGHT - 1), j);
            else
                throw new ArgumentOutOfRangeException("Not implement Step");

            if( (action == ACTION_DOWN 
                && i == 2 && 1 <= j && j <= 10) 
                || (action == ACTION_RIGHT 
                    && state.Item1 == START.Item1 
                    && state.Item2 == START.Item2) )
            {
                reward = -100;
                next_state = START;
            }

            return ((next_state.Item1, next_state.Item2), reward);
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

        public double Sarsa(Dictionary<(int, int, int), double> q_value, 
            bool expected = false)
        {
            var state = START;
            var action = ChooseAction(state, q_value);
            var rewards = 0.0;
            while (state.Item1 != GOAL.Item1 
                || state.Item2 != GOAL.Item2)
            {
                ((int, int) next_state, double reward) = Step(state, action);
                var next_action = ChooseAction(next_state, q_value);
                rewards += reward;
                var target = 0.0;
                if (!expected)
                {
                    target = q_value[(next_state.Item1, next_state.Item2, next_action)];
                }
                else
                {
                    var q_next = q_value
                        .Where( x => x.Key.Item1 == next_state.Item1 && x.Key.Item2 == next_state.Item2)
                        .ToList();
                    var best_actions = q_next.Where(x => x.Value == q_next.Max(y => y.Value)).Select( z => z.Key.Item3);
                    foreach (var action_ in ACTIONS)
                    {
                        if (best_actions.Contains(action_))
                        {
                            target += ( (1.0 - EPSILON) / best_actions.Count() + EPSILON / ACTIONS.Count()) * q_value[(next_state.Item1, next_state.Item2, action_)];
                        }
                        else
                        {
                            target += EPSILON / ACTIONS.Count() * q_value[(next_state.Item1, next_state.Item2, action_)];
                        }
                        
                    }
                }
                target *= GAMMA;
                q_value[(state.Item1, state.Item2, action)] +=
                    ALPHA * (reward + target - q_value[(state.Item1, state.Item2, action)]);
                state = next_state;
                action = next_action;
            }
            return rewards;
        }

        public double QLearning(Dictionary<(int, int, int), double> q_value)
        {
            var state = START;
            double rewards = 0.0;
            while (state.Item1 != GOAL.Item1
                || state.Item2 != GOAL.Item2)
            {
                var action = ChooseAction(state, q_value);
                ((int, int) next_state, double reward) = Step(state, action);
                rewards += reward;
                var q_next = q_value
                        .Where(x => x.Key.Item1 == next_state.Item1 && x.Key.Item2 == next_state.Item2)
                        .ToList();
                q_value[(state.Item1, state.Item2, action)] += ALPHA
                    * (reward + GAMMA * q_next.Max(x => x.Value) - q_value[(state.Item1, state.Item2, action)]);
                state = next_state;
            }
            return rewards;
        }

        [Fact]
        public void figure_6_4()
        {
            var episodes = 500;
            var runs = 50;

            var rewards_sarsa = new List<double>();
            var rewards_q_learning = new List<double>();
            for (int i = 0; i < runs; i++)
            {
                rewards_sarsa.Add(0.0);
                rewards_q_learning.Add(0.0);
            }

            for (int i = 0; i < runs; i++)
            {
                var q_sarsa = get_q_table();
                var q_q_learning = get_q_table();
                for (int j = 0; j < episodes; j++)
                {
                    rewards_sarsa[i] += Sarsa(q_sarsa);
                    rewards_q_learning[i] += QLearning(q_q_learning);
                }
            }

            for (int i = 0; i < runs; i++)
            {
                rewards_sarsa[i] = rewards_sarsa[i] / runs;
                rewards_q_learning[i] = rewards_q_learning[i] / runs;
            }

            for (int i = 0; i < runs; i++)
            {
                console.WriteLine($"{rewards_sarsa[i]}, {rewards_q_learning[i] }");
            }
        }

        Dictionary<(int, int, int), double> get_q_table()
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
            return q_value;
        }
    }
}
