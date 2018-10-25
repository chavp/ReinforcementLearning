using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public class DynaParams
    {
        public static readonly double gamma = 0.95;
        public static readonly double epsilon = 0.1;
        public static readonly double alpha = 0.1;
        public static readonly int time_weight = 0;
        public static int planning_steps = 5;
        public static readonly int runs = 10;
        public static readonly List<string> methods = new List<string>
        {
            "Dyna-Q", "Dyna-Q+"
        };
        public static readonly int theta = 0;
    }
}
