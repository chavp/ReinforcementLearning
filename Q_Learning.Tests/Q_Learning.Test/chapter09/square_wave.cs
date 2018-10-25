using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace Q_Learning.Test.chapter09
{
    public class square_wave
    {
        ITestOutputHelper console;
        public square_wave(ITestOutputHelper console)
        {
            this.console = console;
        }

        public static Interval DOMAIN = new Interval(0.0, 2.0);

        public double SquareWave(double x)
        {
            if (0.5 < x && x < 1.5)
                return 1.0;
            return 0.0;
        }

        Random Ran = new Random();

        public List<(double, double)> sample(int n)
        {
            var samples = new List<(double, double)>();
            for (int i = 0; i < n; i++)
            {
                var x = Ran.NextDouble() * DOMAIN.Size;
                var y = SquareWave(x);
                samples.Add((x, y));
            }
            return samples;
        }

        public void approximate(
            List<(double, double)> samples,
            ValueFunction value_function)
        {
            foreach ((double x, double y) in samples)
            {
                var delta = y - value_function.value(x);
                value_function.update(delta, x);
            }
        }

        [Fact]
        public void figure_9_8()
        {
            var num_of_samples = new int[] { 10, 40, 160, 640, 2560, 10240 };
            var feature_widths = new double[] { 0.2, 0.4, 1.0 };
            var axis_x = new List<double>();
            for (double i = DOMAIN.Left; i < DOMAIN.Right; i+= 0.02)
            {
                axis_x.Add(i);
            }
            for (int indexer = 0; indexer < num_of_samples.Length; indexer++)
            {
                var num_of_sample = num_of_samples[indexer];
                var samples = sample(num_of_sample);
                var value_functions = feature_widths.Select(feature_width =>
                {
                    return new ValueFunction(feature_width, DOMAIN);
                }).ToList();
                foreach (var value_function in value_functions)
                {
                    approximate(samples, value_function);
                    var values = axis_x.Select(x =>
                    {
                        return value_function.value(x);
                    }).ToList();

                    plot(axis_x, values, value_function.FeatureWidth, num_of_sample);
                    console.WriteLine($" --------------------------------- ");
                }
            }
        }

        public void plot(
            List<double> axis_x,
            List<double> values,
            double feature_width,
            int num_of_sample)
        {
            var csv = new StringBuilder();
            csv.AppendLine("x,y");
            //console.WriteLine($" feature_width = {feature_width} ");
            for (int i = 0; i < axis_x.Count; i++)
            {
                csv.AppendLine($"{axis_x[i]},{values[i]}");
            }

            string folder = @"D:\workspace\projects\RL\Q_Learning.Tests\Q_Learning.Test\chapter09";
            File.WriteAllText(
                Path.Combine(folder, $"{num_of_sample}_{feature_width}.csv"), csv.ToString());
        }
    }
}
