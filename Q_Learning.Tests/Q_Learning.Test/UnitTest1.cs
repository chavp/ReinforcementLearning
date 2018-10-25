using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    public class UnitTest1
    {
        ITestOutputHelper console;
        public UnitTest1(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Fact]
        public void Test1()
        {
            // http://incompleteideas.net/
            // http://gym.openai.com/
            // http://mnemstudio.org/path-finding-q-learning-tutorial.htm
            // https://medium.com/@thebear19/reinforcement-learning-rl-101-q-learning-e84c2e92d448
            // https://en.wikipedia.org/wiki/Q-learning
            var env = new CliffWalking(4, 4);
            //var agent = new QAgent(env.ObservationSpace, env.ActionSpaceSize);

            var rewards = new List<double>();
            foreach (var episode in Enumerable.Range(1, 200))
            {
                rewards.Clear();
                
                var state = env.Reset();
                console.WriteLine($"Start episode [{episode}] Environment <Reset> agent ({state})");
                env.RenderEnv(console);
                console.WriteLine($" ");

                for (int step = 1; step <= 200; step++)
                {
                    //var action = agent.ChooseAction(state);

                    (int currenState, int action, string info) obs = env.ActionSpaceSample();

                    (int nextState, double reward, bool done, string info) feedback = env.Step(obs.action);
                    //agent.Learn(state, action, feedback.nextState, feedback.reward, feedback.done);

                    rewards.Add(feedback.reward);

                    //state = feedback.nextState;
                    console.WriteLine($"-----> Step [{step}], Agent action <{env.MapAction(obs.action)}> ");
                    env.RenderEnv(console);
                    if (feedback.done)
                    {
                        //agent.ReduceExploration(episode);
                        console.WriteLine($"End episode [{episode}], Total step {step},  Average reward {rewards.Average()}");
                        console.WriteLine($" ");

                        break;
                    }

                    console.WriteLine($" ");
                }

                //console.WriteLine($"End episode [{episode}], Total step: {rewards.Count}, Total rewards: {rewards.Sum()}");
            }

            env.RenderQ(console);
        }

        [Fact]
        public void Test2()
        {
            Func<int, bool> odd = (n) => (n % 2 > 0);
            Func<int, bool> even = (n) => (n % 2 == 0);
            List<Func<int, bool>> actions = new List<Func<int, bool>>
            {
                odd, even
            };

            Dictionary<(int, int), double> Q = new Dictionary<(int, int), double>
            {

            };

            for (int s = 0; s < 100; s++)
            {
                for (int a = 0; a < 2; a++)
                {
                    Q.Add((s, a), 0);
                }
            }

            double ep = 0.1;
            Random ran = new Random();
            Func<int, int> act = (s) =>
            {
                if(ran.NextDouble() < ep)
                {
                    return ran.Next(0, actions.Count);
                }

                var A_s = Q.Where( x => x.Key.Item1 == s).ToList();
                var maxRaList = A_s.Where(x => x.Value == A_s.Max(y => y.Value)).Select(z => z.Key.Item2).ToList();
                var maxRa = maxRaList[ran.Next(0, maxRaList.Count)];
                return maxRa;
            };

            Func<int, int, (int, double, bool)> step = (s, a) =>
            {
                double reward = 0;
                int nextS = s + 1;
                if (nextS > 99)
                    return (s, reward, true);
                reward = actions[a](s) ? 1 : 0;
                Q[(s, a)] = 0.8 * reward + 0.2 * Q[(nextS, a)];
                return (nextS, reward, false);
            };

            for (int e = 0; e < 100; e++)
            {
                double totalReward = 0.0;
                int s = 0;
                for (int t = 0; t < 1000; t++)
                {
                    var a = act(s);
                    var obs = step(s, a);

                    totalReward += obs.Item2;
                    s = obs.Item1;
                    if (obs.Item3)
                    {
                        //console.WriteLine($"{t}, totalReward = {totalReward}");
                        break;
                    }
                }
            }

            foreach (var item in Q)
            {
                console.WriteLine($"({item.Key.Item1}, {item.Key.Item2}) = {item.Value}");
            }
        }

        [Fact]
        public void Test3()
        {
            var actions = new List<string>
            {
                "L", "R"
            };

            bool l_policy = true;
            Func<int, int> act = (s1) =>
            {
                if (l_policy && s1 == 0) return 0;
                if (!l_policy && s1 == 0) return 1;
                return 0;
            };

            

            Func<int, int, (int, double)> step = (s2, a) =>
             {
                 if (s2 == 0 && a == 0) return (1, 1);
                 if (s2 == 0 && a == 1) return (2, 0);

                 if (s2 == 1) return (0, 0);
                 if (s2 == 2) return (0, 2);

                 return (0, 0);
             };

            
            List<double> lamdas = new List<double>
            {
                0, 0.9, 0.5
            };

            for (int p = 0; p < 2; p++)
            {
                foreach (var lamda in lamdas)
                {
                    var s = 0;
                    var values = new List<double>
                    {
                        0, 0, 0
                    };
                    for (int i = 0; i < 100; i++)
                    {
                        (int nextS, double reward) ops = step(s, p);
                        values[s] = ops.reward + lamda * values[ops.nextS];
                        s = ops.nextS;
                    }

                    console.WriteLine($"policy = {p}, lamda = {lamda}, rewards = {values.Sum()}");
                }
            }
        }


        [Fact]
        public void Test4()
        {
            var counter = Counter.Instance;

            var dics = new ConcurrentDictionary<int, int>();
            Enumerable.Range(0, 1000).AsParallel().ForAll(i =>
            {
                var newId = Counter.Instance.NextID;
                console.WriteLine($"{newId}");
                dics.TryAdd(newId, 0);
            });

            //for (int i = 0; i < 1000; i++)
            //{
            //    var nid = Counter.Instance.NextID;
            //    console.WriteLine($"{nid}");
            //    dics.TryAdd(nid, 0);
            //}

            Assert.Equal(1000, dics.Keys.Count);
        }
    }

    public class Counter
    {
        private static Counter aCounter = null;
        private Counter() { }
        public static Counter Instance => aCounter ?? (aCounter = new Counter());
        private int _nextID = 0;
        public int NextID => Interlocked.Increment(ref _nextID);
        //public int NextID => ++nextID;
    }
}
