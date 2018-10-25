using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static System.Console;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace RL.Tests
{
    // http://incompleteideas.net/
    // http://incompleteideas.net/book/ebook/node1.html

    public class UnitTest1
    {
        int MAX_EPISODES = 13; // maximum episodes

        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void treasure_on_right()
        {
            var env = new TreasureOnRightEnv();
            var rewards = new List<double>();
            foreach (var episode in Enumerable.Range(0, MAX_EPISODES))
            {
                int stepCounter = 0;
                int state = 0;
                bool isTerminated = false;
                env.Render(state, episode, stepCounter, isTerminated, output);

                while (!isTerminated)
                {
                    var action = env.ChooseAction(state);
                    ++stepCounter;
                    var (nextState, reward, done) = env.Step(state, action);
                    env.Learn(state, action, nextState, reward, done);
                    isTerminated = done;
                    state = nextState;
                    env.Render(state, episode, stepCounter, isTerminated, output);
                    rewards.Add(reward);
                }
            }

            output.WriteLine($"Average rewards = {rewards.Average()}");
        }

        [Fact]
        public void maze()
        {

        }
    }

    public class DataFrame<X, Y>
    {
        Dictionary<(X, Y), double> qDics = new Dictionary<(X, Y), double>();
        public DataFrame(Dictionary<(X, Y), double> qDics)
        {
            this.qDics = qDics;
        }

        public double GetValue(X x, Y y)
        {
            return qDics[(x, y)];
        }

        public void SetValue(X x, Y y, double value)
        {
            qDics[(x, y)] = value;
        }

        public void IncrementValue(X x, Y y, double value)
        {
            qDics[(x, y)] += value;
        }

        public double GetMax(X x)
        {
            return (from d in qDics
                    where d.Key.Item1.Equals(x)
                    select d.Value).Max();
        }

        public double GetMax(Y y)
        {
            return (from d in qDics
                    where d.Key.Item1.Equals(y)
                    select d.Value).Max();
        }

        public DataFrame<X, Y> Where(Func<KeyValuePair<(X, Y), double>, bool> predicate)
        {
            var xyDics = qDics.Where(predicate);
            var newDics = new Dictionary<(X, Y), double>();
            foreach (var item in xyDics)
            {
                newDics.Add(item.Key, item.Value);
            }
            return new DataFrame<X, Y>(newDics);
        }

        public static DataFrame<X, Y> Build(IEnumerable<X> xList, IEnumerable<Y> yList)
        {
            var qDics = new Dictionary<(X, Y), double>();
            foreach (var x in xList.Distinct())
            {
                foreach (var y in yList.Distinct())
                {
                    qDics.Add((x, y), default(double));
                }
            }
            return new DataFrame<X, Y>(qDics);
        }
    }
}
