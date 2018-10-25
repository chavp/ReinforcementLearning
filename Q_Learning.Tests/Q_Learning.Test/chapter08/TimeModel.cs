using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public class TimeModel
    {
        double Time_weigth = 0.0004;
        int Time = 0;
        Random Ran = new Random();
        Dictionary<((int, int), int), ((int, int), double, int)> model;
        Maze maze = new Maze();

        public void feed((int, int) state, int action, 
            (int, int) next_state, double reward)
        {
            ++Time;
            foreach (var action_ in Maze.actions)
            {
                if(action_ != action)
                {
                    if(!model.ContainsKey((state, action_)))
                    {
                        model.Add((state, action_), (state, 0, 1));
                    }
                }
            }

            if (!model.ContainsKey((state, action)))
            {
                model.Add((state, action), (next_state, reward, Time));
            }

            model[(state, action)] = (next_state, reward, Time);
        }

        public ((int, int) state, int action, (int, int) next_state, double reward) sample()
        {
            int state_action_index = Ran.Next(0, model.Count);
            ((int, int) state, int action) = model.Keys.ToList()[state_action_index];

            ((int, int) next_state, double reward, int time) = model[(state, action)];

            reward += Time_weigth * Math.Sqrt(this.Time - time);

            return (state, action, next_state, reward);
        }
    }
}
