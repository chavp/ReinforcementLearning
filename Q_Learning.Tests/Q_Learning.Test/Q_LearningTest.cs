using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    public abstract class Q_LearningTest
    {
        public ITestOutputHelper Console { get; private set; }
        public Q_LearningTest(ITestOutputHelper console)
        {
            this.Console = console;
        }
    }
}
