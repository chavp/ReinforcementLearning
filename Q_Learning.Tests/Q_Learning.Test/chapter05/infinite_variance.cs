using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit.Abstractions;
using Xunit;

namespace Q_Learning.Test.chapter05
{
    public class infinite_variance
    {
        ITestOutputHelper console;
        public infinite_variance(ITestOutputHelper console)
        {
            this.console = console;
        }

        static readonly int ActionBack = 0;
        static readonly int ActionEnd = 1;

        Random ran = new Random();

        int behavior_policy()
        {
            return ran.Next(0, 2);
        }

        int target_policy()
        {
            return ActionBack;
        }

        (double, List<int>) Play()
        {
            var trajectory = new List<int>();
            while (true)
            {
                var action = behavior_policy();
                trajectory.Add(action);
                if (action == ActionEnd)
                    return (0, trajectory);
                if (ran.NextDouble() > 0.9)
                    return (1, trajectory);
            }
        }

        [Fact]
        public void figure_5_4()
        {
            var runs = 10;
            var episodes = 100000;
            var rho = 0.0;
            
            for (int i = 0; i < 1; i++)
            {
                var acc_rewards = new List<(int, double)>();
                var rewards = new List<double>();

                for (int j = 0; j < episodes; j++)
                {
                    (double reward, List<int> trajectory) = Play();
                    if(trajectory.Last() == ActionEnd)
                    {
                        rho = 0.0;
                    }
                    else
                    {
                        rho = 1.0 / Math.Pow(0.5, trajectory.Count);
                    }
                    rewards.Add(rho * reward);
                    acc_rewards.Add((j+1, rewards.Sum()));
                }

                //foreach ((int ep, double reward) in acc_rewards)
                //{
                //    console.WriteLine($"{ep}, {reward}");
                //}
            }

            //foreach (var acc_reward in acc_rewards)
            //{
            //    console.WriteLine($"{acc_reward}");
            //}
        }
    }
}
