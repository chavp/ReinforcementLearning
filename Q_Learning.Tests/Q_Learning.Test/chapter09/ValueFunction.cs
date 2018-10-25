using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter09
{
    public class ValueFunction
    {
        public double FeatureWidth { get; }
        public int NumOfFeatrues { get; }
        public List<Interval> Features { get; }
        public double Alpha { get; }
        public Interval Domain { get; }
        public List<double> Weights { get; }

        public ValueFunction(
            double feature_width,
            Interval domain,
            double alpha = 0.2,
            int num_of_features = 50
            )
        {
            FeatureWidth = feature_width;
            NumOfFeatrues = num_of_features;
            Features = new List<Interval>();
            Alpha = alpha;
            Domain = domain;

            var step = (domain.Size - feature_width) / (num_of_features - 1);
            var left = domain.Left;
            for (int i = 0; i < num_of_features - 1; i++)
            {
                Features.Add(new Interval(left, left + feature_width));
                left += step;
            }
            Features.Add(new Interval(left, domain.Right));

            Weights = Enumerable
                .Range(0, num_of_features)
                .Select( x => 0.0).ToList();
            
        }

        public List<int> get_active_features(double x)
        {
            var active_features = new List<int>();
            for (int i = 0; i < Features.Count; i++)
            {
                if (Features[i].contain(x))
                    active_features.Add(i);
            }
            return active_features;
        }

        public double value(double x)
        {
            var active_features = get_active_features(x);
            var sum = 0.0;
            foreach (var index in active_features)
            {
                sum += Weights[index];
            }
            return sum;
        }

        public void update(double delta, double x)
        {
            var active_features = get_active_features(x);
            delta *= Alpha / active_features.Count;
            foreach (var index in active_features)
            {
                Weights[index] += delta;
            }
        }
    }
}
