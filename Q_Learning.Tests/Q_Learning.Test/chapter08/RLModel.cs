using System;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public abstract class RLModel
    {
        protected Dictionary<((int, int), int), ((int, int), double)> model;

        public abstract void feed((int, int) state, int action, (int, int) next_state, double reward);
        public abstract ((int, int) state, int action, (int, int) next_state, double reward) sample();
        public abstract (int, (int, int), int, (int, int), double) sample_pri();

        public abstract void insert(int priority, (int, int) state, int action);

        public abstract bool empty();

        public abstract List<((int, int), int, double)> predecessor((int, int) state);

        public RLModel()
        {
            model = new Dictionary<((int, int), int), ((int, int), double)>();
        }
    }
}
