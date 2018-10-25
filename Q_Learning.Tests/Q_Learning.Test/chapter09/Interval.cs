using System;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter09
{
    public class Interval
    {
        public double Left { get;  }
        public double Right { get; }

        public Interval(double left, double right)
        {
            Left = left;
            Right = right;
        }

        public bool contain(double x)
        {
            return (Left <= x && x < Right);
        }

        public double Size
        {
            get
            {
                return Right - Left;
            }
        }
    }
}
