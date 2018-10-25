using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    using MathNet.Numerics.LinearAlgebra;
    using Xunit;

    public class grid_world : Q_LearningTest
    {
        public grid_world(ITestOutputHelper console)
            : base(console) { }

        int WorldSize = 5;
        (int h, int w) APos = (0, 1);
        (int h, int w) APrimePos = (4, 1);
        (int h, int w) BPos = (0, 3);
        (int h, int w) BPrimePos = (2, 3);
        double Discount = 0.9;
        // left, up, right, down
        List<(int, int)> Actions = new List<(int, int)>
        {
            (0, -1),
            (-1, 0),
            (0, 1),
            (1, 0),
        };
        double ActionsProb = 0.25;

        public ((int h, int w) nextState, double reward) Step((int h, int w) state, (int h, int w) action)
        {
            if (state.Equals(APos))
                return (APrimePos, 10);

            if (state.Equals(BPos))
                return (BPrimePos, 5);

            double rReward = 0.0;
            (int h, int w) rNextState = (state.h + action.h, state.w + action.w);
            if(rNextState.h < 0 || rNextState.h >= WorldSize 
                || rNextState.w < 0 || rNextState.w >= WorldSize)
            {
                rReward = -1;
                rNextState = state;
            }

            return (rNextState, rReward);
        }

        public void PrintWorld(Matrix<double> states)
        {
            Console.WriteLine(states.ToMatrixString());
        }

        public Matrix<double> Zeros(int worldSize)
        {
            return Matrix<double>.Build.Dense(worldSize, worldSize);
        }

        [Fact]
        public void figure_3_2()
        {
            var value = Zeros(WorldSize);

            while (true)
            {
                var newValue = Zeros(WorldSize);
                for (int i = 0; i < WorldSize; i++)
                {
                    for (int j = 0; j < WorldSize; j++)
                    {
                        foreach (var action in Actions)
                        {
                            ((int nextH, int nextW) pos, double reward) obs = Step((i, j), action);

                            newValue[i, j] += ActionsProb * (obs.reward + Discount * value[obs.pos.nextH, obs.pos.nextW]);
                        }
                    }
                }
                //PrintWorld(newValue);

                var diffM = Matrix<double>.Abs(value - newValue);
                var diff = diffM.RowSums().Sum();
                if(diff < 0.0004)
                {
                    PrintWorld(newValue);
                    break;
                }
                value = newValue;
            }
        }

        [Fact]
        public void figure_3_5()
        {
            var value = Zeros(WorldSize);

            while (true)
            {
                var newValue = Zeros(WorldSize);
                for (int i = 0; i < WorldSize; i++)
                {
                    for (int j = 0; j < WorldSize; j++)
                    {
                        var values = new List<double>();
                        foreach (var action in Actions)
                        {
                            ((int nextH, int nextW) pos, double reward) obs = Step((i, j), action);
                            values.Add(obs.reward + Discount * value[obs.pos.nextH, obs.pos.nextW]);

                            newValue[i, j] = values.Max();
                        }
                    }
                }
               // PrintWorld(newValue);

                var diffM = Matrix<double>.Abs(value - newValue);
                var diff = diffM.RowSums().Sum();
                if (diff < 0.0004)
                {
                    PrintWorld(newValue);
                    break;
                }
                value = newValue;
            }
        }
    }
}
