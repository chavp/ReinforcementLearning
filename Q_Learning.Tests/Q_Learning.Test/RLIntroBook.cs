using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    public class RLIntroBook
    {
        ITestOutputHelper console;
        public RLIntroBook(ITestOutputHelper console)
        {
            this.console = console;
        }


        // https://github.com/ShangtongZhang/reinforcement-learning-an-introduction
        [Fact]
        public void Test1()
        {
            Matrix<double> A = Matrix.Build.Dense(10, 10);
            A[9, 9] = 2;
            console.WriteLine($"{A.ToMatrixString()}");
        }
    }
}
