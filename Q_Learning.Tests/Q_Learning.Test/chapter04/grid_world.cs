using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter04
{
    using static Q_Learning.Test.helpers.Helpers;

    public class grid_world
    {
        ITestOutputHelper console;
        public grid_world(ITestOutputHelper console)
        {
            this.console = console;
        }

        readonly static int WorldSize = 4;
        readonly static double ActionProb = 0.25;
        // left, up, right, down
        readonly static List<(int, int)> Actions = new List<(int, int)>
        {
            (0, -1),
            (-1, 0),
            (0, 1),
            (1, 0),
        };

        static Func<(int h, int w), bool> IsTerminated = (state) =>
        {
            return (state.h == 0 && state.w == 0)
            || (state.w == WorldSize - 1 && state.h == WorldSize - 1);
        };

        static Func<(int h, int w), (int h, int w), ((int h, int w) nextState, double reward)> Step =
            (state, action) =>
            {
                (int h, int w) nextState = (state.h + action.h, state.w + action.w);
                if(nextState.w < 0 
                    || nextState.w >= WorldSize
                    || nextState.h < 0
                    || nextState.h >= WorldSize)
                {
                    nextState = state;
                }

                var reward = -1;
                return (nextState, reward);
            };

        static Func<bool, (Dictionary<(int, int), double>, int)> ComputeStateValue =
            (inPlace) =>
            {
                var newStateValues = new Dictionary<(int, int), double>();
                var stateValues = new Dictionary<(int, int), double>();
                ForEachLoop(WorldSize, WorldSize,
                (i, j) =>
                {
                    newStateValues.Add((i, j), 0.0);
                    stateValues.Add((i, j), 0.0);
                });

                int iteration = 1;
                while (true)
                {
                    var src = (inPlace) ? newStateValues : stateValues;
                    ForEachLoop(WorldSize, WorldSize,
                    (i, j) =>
                        {
                            if (!IsTerminated((i, j)))
                            {
                                var value = 0.0;
                                foreach (var action in Actions)
                                {
                                    ((int nextI, int nextJ) next,double reward) obs = Step((i, j), action);
                                    value += ActionProb * (obs.reward + src[(obs.next)]);
                                }
                                newStateValues[(i, j)] = value;
                            }
                        });

                    var totalDiff = Diff(newStateValues, stateValues);

                    Copy(newStateValues, stateValues);

                    if (totalDiff < 0.0004)
                    {
                        break;
                    }

                    iteration += 1;
                }

                return (stateValues, iteration);
            };

        [Fact]
        public void figure_4_1()
        {
            (Dictionary<(int, int), double> values, int sync_iteration) results1 = 
                ComputeStateValue(false);

            (Dictionary<(int, int), double> _, int asycn_iteration) results2 =
                ComputeStateValue(true);

            console.WriteLine($"In-place: {results2.asycn_iteration}");
            console.WriteLine($"Synchronous: {results1.sync_iteration}");
        }
    }
}
