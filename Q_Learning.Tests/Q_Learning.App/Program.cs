using Q_Learning.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Console;

namespace Q_Learning.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var env = new CliffWalking(5, 5);
            var rewards = new List<double>();
            foreach (var episode in Enumerable.Range(1, 1000))
            {
                WriteLine($" Press ENTER for start episode [{episode}] ");
                //Console.ReadLine();

                Clear();
                rewards.Clear();
                
                var state = env.Reset();
                WriteLine($"Start episode [{episode}] Environment.Reset() Agent({state})");
                env.RenderEnv();
                WriteLine($" ");
                Thread.Sleep(TimeSpan.FromSeconds(1));

                bool isDone = false;
                int step = 0;
                while (!isDone)
                {
                    //var action = agent.ChooseAction(state);

                    (int currentState, int action, string info) obs = env.ActionSpaceSample();

                    ++step;
                    (int nextState, double reward, bool done, string info) feedback = env.Step(obs.action);
                    //agent.Learn(state, action, feedback.nextState, feedback.reward, feedback.done);

                    rewards.Add(feedback.reward);
                    Clear();
                    //state = feedback.nextState;
                    WriteLine($"Episode [{episode}] --> Step [{step}], Agent({obs.currentState}, {obs.info}).Action({env.MapAction(obs.action)}) ");
                    env.RenderEnv();

                    isDone = feedback.done;
                    if (feedback.done)
                    {
                        //env.ReduceExploration(1);
                        WriteLine($"End episode [{episode}], Total step {step},  Average reward {rewards.Average()}");
                        //WriteLine($"--------------- Q-Table ---------------");
                        //env.RenderQ();
                        //WriteLine($" ");

                        break;
                    }

                    WriteLine($" ");

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    Clear();
                }

                //console.WriteLine($"End episode [{episode}], Total step: {rewards.Count}, Total rewards: {rewards.Sum()}");
            }

            WriteLine($"--------------- Q-Table ---------------");
            env.RenderQ();
            Console.ReadLine();
        }
    }
}
