using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    using static System.Console;

    public class ProbabilityRandomTest
    {
        ITestOutputHelper console;
        public ProbabilityRandomTest(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Fact]
        public void Test1()
        {
            var dicsOfProbs = new Dictionary<string, double>
            {
                { "A", 0.1},
                { "B", 0.2},
                { "C", 0.4},
                { "D", 0.2},
                { "E", 0.1},
            };

            var probabilityRandom = new ProbabilityRandom<string>();
            foreach (var item in dicsOfProbs)
            {
                probabilityRandom.SetProb(item.Key, item.Value);
            }

            probabilityRandom.SetProb("D", 0.1);
            probabilityRandom.SetProb("F", 0.1);

            var rand = new Random();
            var actualProbs = new Dictionary<string, double>
            {
                { "A", 0},
                { "B", 0},
                { "C", 0},
                { "D", 0},
                { "E", 0},
                { "F", 0},
            };

            int maxRand = 50000;
            for (int i = 0; i < maxRand; i++)
            {
                var num = probabilityRandom.Next();

                actualProbs[num] = actualProbs[num] + 1;
            }

            double sum = 0;
            foreach (var item in actualProbs)
            {
                console.WriteLine($"P({item.Key}) = {item.Value / maxRand} ({probabilityRandom.DicOfProbs[item.Key]})");

                sum += item.Value / maxRand;
            }

            console.WriteLine($"P = {sum}");

        }

        [Fact]
        public void TestSayHiTo()
        {
            Action<string> sayHiTo = (someone) => WriteLine($"Hi {someone}");
            sayHiTo("roof");

        }
    }
}
