using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    public class ten_armed_testbed
    {
        ITestOutputHelper console;
        public ten_armed_testbed(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Fact]
        public void Test1()
        {
            var bandits = new List<Bandit>
            {
                new Bandit(epsilon: 0.2, initial: 0, stepSize: 0.1, gradient: true),
                new Bandit(epsilon: 0.2, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.2, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.3, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.4, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.5, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.6, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.7, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.8, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 0.9, initial: 0, stepSize: 0.1),
                //new Bandit(epsilon: 1, initial: 0, stepSize: 0.1),
            };
            var results = Simulate(2000, 1000, bandits);
            foreach (var item in results)
            {
                var bandit = bandits[item.Key];
                console.WriteLine(
                    $"Bandit: (Epsilon = {bandit.Epsilon}, Initial = {bandit.Initial}, StepSize = {bandit.StepSize}) (BestActionCount = {item.Value.Item1}, AvgReward = {item.Value.Item2}, Greedy = {item.Value.Item3}) ");
            }
        }

        public Dictionary<int, (double, double, double)> Simulate(int runs, int times, List<Bandit> bandits)
        {
            var bestActionCount = new Dictionary<(int, int, int), int>();
            var rewards = new Dictionary<(int, int, int), double>();

            var bestBndits = new Dictionary<int, (double, double, double)>();
            for (int i = 0; i < bandits.Count; i++)
            {
                double totalRewards = 0;
                var totalBestAction = 0;
                double totalGreedy = 0;
                bestBndits.Add(i, (0, 0.0, totalGreedy));
                var bandit = bandits[i];
                //bandit.Reset();
                for (int run = 0; run < runs; run++)
                {
                    bandit.Reset();
                    for (int t = 0; t < times; t++)
                    {
                        (int action, bool greedy) act = bandit.Act();
                        var reward = bandit.Step(act.action);
                        //rewards.Add((i, run, t), reward);
                        bestBndits[i] = (bestBndits[i].Item1, bestBndits[i].Item2 + reward, totalGreedy);
                        ++totalRewards;
                        if (act.action == bandit.BestAction)
                        {
                            ++totalBestAction;
                            bestBndits[i] = (bestBndits[i].Item1 + act.action, bestBndits[i].Item2, totalGreedy);
                        }

                        if (act.greedy) ++totalGreedy;
                    }
                }
                
                bestBndits[i] = (bestBndits[i].Item1 / totalBestAction, bestBndits[i].Item2 / totalRewards, totalGreedy / totalRewards);
            }


            return bestBndits;
        }
    }
}
