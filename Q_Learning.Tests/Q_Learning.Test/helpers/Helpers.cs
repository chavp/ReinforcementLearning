using System;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.helpers
{
    public static class Helpers
    {
        public static Action<int, int, Action<int, int>> ForEachLoop = (I, J, act) =>
        {
            for (int i = 0; i < I; i++)
            {
                for (int j = 0; j < J; j++)
                {
                    act?.Invoke(i, j);
                }
            }
        };

        public static T Diff<K, T>(Dictionary<K, T> x, Dictionary<K, T> y)
            where T: struct
        {
            T totalDiff = default(T);
            foreach (var item in x)
            {
                totalDiff += Math.Abs((dynamic)item.Value - (dynamic)y[item.Key]);
            }
            return totalDiff;
        }

        public static void Copy<K, T>(Dictionary<K, T> from, Dictionary<K, T> to)
        {
            foreach (var item in from)
            {
                to[item.Key] = item.Value;
            }
        }
    }
}
