using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public class TrivialModel : RLModel
    {
        //protected Dictionary<((int, int), int), ((int, int), double)> model;

        public TrivialModel()
        {
            model = new Dictionary<((int, int), int), ((int, int), double)>();
        }

        public override void feed((int, int) state, int action, (int, int) next_state, double reward)
        {
            if(!model.ContainsKey((state, action)))
            {
                model.Add((state, action), (next_state, reward));
            }
        }

        Random Ran = new Random();
        public override ((int, int) state, int action, (int, int) next_state, double reward) sample()
        {
            var state_action_index = Ran.Next(0, model.Count);
            ((int, int) state, int action) = model.Keys.ToList()[state_action_index];
            ((int, int) next_state, double reward) = model[(state, action)];

            return (state, action, next_state, reward);
        }

        public override void insert(int priority, (int, int) state, int action)
        {
            throw new NotImplementedException();
        }

        public override bool empty()
        {
            throw new NotImplementedException();
        }

        public override List<((int, int), int, double)> predecessor((int, int) state)
        {
            throw new NotImplementedException();
        }

        public override (int, (int, int), int, (int, int), double) sample_pri()
        {
            throw new NotImplementedException();
        }
    }
}
