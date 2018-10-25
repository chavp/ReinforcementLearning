using System;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public class PriorityModel : TrivialModel
    {
        PriorityQueue Priority_queue = new PriorityQueue();
        Dictionary<(int, int), List<((int, int), int)>> Predecessors = new Dictionary<(int, int), List<((int, int), int)>>();
        public override void insert(int priority, (int, int) state, int action)
        {
            Priority_queue.add_item((state, action), -priority);
        }

        public override bool empty()
        {
            return Priority_queue.empty();
        }

        public override (int, (int, int), int, (int, int), double) sample_pri()
        {
            (((int, int) state, int action), int priority) = Priority_queue.pop_item();
            ((int, int) next_state, double reward) = model[(state, action)];
            return (-priority, state, action, next_state, reward);
        }

        public override void feed((int, int) state, int action, (int, int) next_state, double reward)
        {
            base.feed(state, action, next_state, reward);
            if (!Predecessors.ContainsKey(next_state))
            {
                Predecessors.Add(next_state, new List<((int, int), int)>());
            }

            Predecessors[next_state].Add((state, action));
        }

        public override List<((int, int), int, double)> predecessor((int, int) state)
        {
            var predecessors = new List<((int, int), int , double)>();
            foreach ((var state_pre, var action_pre) in Predecessors[state])
            {
                ((int, int) this_state, double reward) = model[(state_pre, action_pre)];
                predecessors.Add((state_pre, action_pre, reward));
            }
            return predecessors;
        }
    }
}
