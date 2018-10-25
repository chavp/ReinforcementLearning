using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter08
{
    public class test_maze
    {
        ITestOutputHelper console;
        public test_maze(ITestOutputHelper console)
        {
            this.console = console;
        }
        
        Random Ran = new Random();
        public int ChooseAction((int, int) state, Dictionary<(int, int, int), double> q_value)
        {
            if (Ran.NextDouble() < DynaParams.epsilon)
            {
                return Maze.actions[Ran.Next(0, Maze.actions.Count)];
            }
            else
            {
                var values_ = q_value.Where(x => x.Key.Item1 == state.Item1
                && x.Key.Item2 == state.Item2).ToList();

                var maxActs = values_.Where(x => x.Value == values_.Max(y => y.Value)).ToList();
                return maxActs[Ran.Next(0, maxActs.Count)].Key.Item3;
            }
        }

        public int dyna_q(
            Dictionary<(int, int, int), double> q_value,
            RLModel model,
            Maze maze
            )
        {
            var state = maze.START_STATE;
            var steps = 0;
            var c = maze.GOAL_STATES.Contains((0, 8));

            while (!maze.GOAL_STATES.Contains(state))
            {
                ++steps;
                var action = ChooseAction(state, q_value);

                ((int, int) next_state, double reward) = maze.step(state, action);

                // Q-Learning update
                var maxReward = q_value.Where(x => x.Key.Item1 == state.Item1 && x.Key.Item2 == state.Item2)
                        .Select(y => y.Value).Max();
                var oldReward = q_value[(state.Item1, state.Item2, action)];
                q_value[(state.Item1, state.Item2, action)] += DynaParams.alpha * (reward + DynaParams.gamma * maxReward - oldReward);

                model.feed(state, action, next_state, reward);

                for (int t = 0; t < DynaParams.planning_steps; t++)
                {
                    ((int, int) state_, int action_, (int, int) next_state_, double reward_) = model.sample();

                    maxReward = q_value.Where(x => x.Key.Item1 == next_state_.Item1 && x.Key.Item2 == next_state_.Item2)
                        .Select(y => y.Value).Max();
                    oldReward = q_value[(state_.Item1, state_.Item2, action_)];
                    q_value[(state_.Item1, state_.Item2, action_)] += DynaParams.alpha * (reward + DynaParams.gamma * maxReward - oldReward);

                }

                state = next_state;

                if(steps > Maze.max_steps)
                {
                    break;
                }
            }

            return steps;
        }

        public int prioritized_sweeping(
            Dictionary<(int, int, int), double> q_value,
            RLModel model,
            Maze maze
            )
        {
            var state = maze.START_STATE;
            var steps = 0;
            var backups = 0;
            while (!maze.GOAL_STATES.Contains(state))
            {
                ++steps;
                var action = ChooseAction(state, q_value);

                ((int, int) next_state, double reward) = maze.step(state, action);

                var maxReward = q_value.Where(x => x.Key.Item1 == next_state.Item1 && x.Key.Item2 == next_state.Item2)
                        .Select(y => y.Value).Max();
                var oldReward = q_value[(state.Item1, state.Item2, action)];

                var priority = Math.Abs(
                    reward + DynaParams.gamma * maxReward - oldReward
                    );

                if(priority > DynaParams.theta)
                {
                    model.insert((int)priority, state, action);
                }

                int planning_step = 0;
                while(planning_step < DynaParams.planning_steps && !model.empty())
                {
                    (int priority_, (int, int) state_, int action_, (int, int) next_state_, double reward_) = model.sample_pri();

                    maxReward = q_value.Where(x => x.Key.Item1 == next_state_.Item1 && x.Key.Item2 == next_state_.Item2)
                        .Select(y => y.Value).Max();
                    oldReward = q_value[(state_.Item1, state_.Item2, action_)];

                    var delta = reward_ + DynaParams.gamma * maxReward - oldReward;
                    q_value[(state_.Item1, state_.Item2, action_)] += DynaParams.alpha * delta;

                    foreach (((int, int) state_pre, int action_pre, double reward_pre) in model.predecessor(state_))
                    {
                        maxReward = q_value.Where(x => x.Key.Item1 == state_.Item1 && x.Key.Item2 == state_.Item2)
                        .Select(y => y.Value).Max();
                        var preReward = q_value[(state_pre.Item1, state_pre.Item2, action_pre)];

                        priority = Math.Abs(
                            reward_pre + DynaParams.gamma * maxReward - preReward
                            );
                        if (priority > DynaParams.theta)
                            model.insert((int)priority, state_pre, action_pre);
                    }

                    planning_step++;
                }

                state = next_state;

                backups += planning_step + 1;
            }
            return backups;
        }

        [Fact]
        public void priority_queue()
        {
            var priorityQueue = new PriorityQueue();

            priorityQueue.add_item(((1, 1), 1));
            priorityQueue.add_item(((1, 1), 2));

            var task = priorityQueue.pop_item();
            task = priorityQueue.pop_item();
            task = priorityQueue.pop_item();
        }

        [Fact]
        public void figure_8_2()
        {
            var dyna_maze = new Maze();
            int runs = 10;
            int episodes = 10;
            List<int> planning_steps = new List<int> { 0, 5, 50 };

            Dictionary<(int, int), double> steps = new Dictionary<(int, int), double>();
            for (int i = 0; i < runs; i++)
            {
                for (int index = 0; index < planning_steps.Count; index++)
                {
                    var planning_step = planning_steps[index];
                    DynaParams.planning_steps = planning_step;

                    var q_value = GetQValue(dyna_maze.q_size);

                    var model = new TrivialModel();
                    for (int ep = 0; ep < episodes; ep++)
                    {
                        var index_ep = (index, ep);
                        if (!steps.ContainsKey(index_ep))
                            steps.Add(index_ep, 0);
                        steps[(index, ep)] += dyna_q(q_value, model, dyna_maze);
                    }
                }
            }

            foreach (var step in steps.Keys)
            {
                steps[step] /= runs;
            }
        }

        Dictionary<(int, int, int), double> GetQValue((int, int, int) q_size)
        {
            var q_value = new Dictionary<(int, int, int), double>();
            for (int i = 0; i < q_size.Item1; i++)
            {
                for (int j = 0; j < q_size.Item2; j++)
                {
                    for (int act = 0; act < q_size.Item3; act++)
                    {
                        q_value.Add((i, j, act), 0.0);
                    }
                }
            }
            return q_value;
        }
    }
}
