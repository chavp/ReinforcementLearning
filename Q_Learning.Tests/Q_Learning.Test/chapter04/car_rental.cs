using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test.chapter04
{
    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra;

    using static Q_Learning.Test.helpers.Helpers;

    public class car_rental
    {
        ITestOutputHelper console;
        public car_rental(ITestOutputHelper console)
        {
            this.console = console;
        }

        static car_rental()
        {
            Actions = new List<int>();
            for (int i = -MaxMoveOfCars; i < MaxMoveOfCars + 1; i++)
            {
                Actions.Add(i);
            }
            PoissonCache = new Dictionary<double, double>();
        }

        readonly static List<int> Actions = null;
        static Dictionary<double, double> PoissonCache = null;

        readonly static int MaxCars = 20;
        readonly static int MaxMoveOfCars = 5;
        readonly static int RentalRequestFirstLoc = 3;
        readonly static int RentalRequestSecondLoc = 4;
        readonly static int ReturnsFirstLoc = 3;
        readonly static int ReturnsSecondLoc = 2;
        readonly static double Discount = 0.9;
        readonly static double RentalCredit = 10.0;
        readonly static double MoveCarCost = 2.0;
        readonly static int PoissonUpperBound = 11;

        static Func<int, double, double> Poisson 
            = (n, lam) =>
        {
            var key = n * 10 + lam;
            if (!PoissonCache.ContainsKey(key))
            {
                PoissonCache[key] = Math.Exp(-lam) * Math.Pow(lam, n) / SpecialFunctions.Factorial(n);
            }
            return PoissonCache[key];
        };

        static Func<(int first, int second), int, Matrix<double>, bool, double> ExpectedReturn 
            = (state, action, stateValue, constantReturnedCars) =>
        {
            var returns = 0.0;

            returns -= MoveCarCost * Math.Abs(action);
            ForEachLoop(PoissonUpperBound, PoissonUpperBound,
                (rentalRequestFirstLoc, rentalRequestSecondLoc) =>
                {
                    // moving cars
                    var numOfCarsFirstLoc = Math.Min(state.first - action, MaxCars);
                    var numOfCarsSecondLoc = Math.Min(state.second + action, MaxCars);

                    // valid rental requests should be less than actual # of cars
                    var realRentalFirstLoc = Math.Min(numOfCarsFirstLoc, rentalRequestFirstLoc);
                    var realRentalSecondLoc = Math.Min(numOfCarsSecondLoc, rentalRequestSecondLoc);

                    // get credits for renting
                    var reward = (realRentalFirstLoc + realRentalSecondLoc) * RentalCredit;
                    numOfCarsFirstLoc -= realRentalFirstLoc;
                    numOfCarsSecondLoc -= realRentalSecondLoc;

                    // probability for current combination of rental requests
                    var prob = Poisson(rentalRequestFirstLoc, RentalRequestFirstLoc)
                    * Poisson(rentalRequestSecondLoc, RentalRequestSecondLoc);

                    if (constantReturnedCars)
                    {
                        var returnedCarsFirstLoc = ReturnsFirstLoc;
                        var returnedCarsSecondLoc = ReturnsSecondLoc;
                        numOfCarsFirstLoc = Math.Min(numOfCarsFirstLoc + returnedCarsFirstLoc, MaxCars);
                        numOfCarsSecondLoc = Math.Min(numOfCarsSecondLoc + returnedCarsSecondLoc, MaxCars);
                        returns += prob * (reward + Discount * stateValue[numOfCarsFirstLoc, numOfCarsSecondLoc]);
                    }
                    else
                    {
                        ForEachLoop(PoissonUpperBound, PoissonUpperBound,
                            (returnedCarsFirstLoc, returnedCarsSecondLoc) =>
                        {
                            var numOfCarsFirstLoc_ = Math.Min(numOfCarsFirstLoc + returnedCarsFirstLoc, MaxCars);
                            var numOfCarsSecondLoc_ = Math.Min(numOfCarsSecondLoc + returnedCarsSecondLoc, MaxCars);

                            var prob_ = Poisson(returnedCarsFirstLoc, ReturnsFirstLoc)
                                * Poisson(returnedCarsSecondLoc, ReturnsSecondLoc)
                                * prob;
                            returns += prob_ * (reward + Discount * stateValue[numOfCarsFirstLoc_, numOfCarsSecondLoc_]);
                        });
                    }
                });

            return returns;
        };

        [Fact]
        public void figure_4_2()
        {
            var value = Matrix<double>.Build.Dense(MaxCars + 1, MaxCars + 1);
            var policy = new Dictionary<(int, int), int>();

            var iterations = 0;

            var newValue = Matrix<double>.Build.Dense(MaxCars + 1, MaxCars + 1);
            var newPolicy = new Dictionary<(int, int), int>();

            ForEachLoop(MaxCars + 1, MaxCars + 1, (i, j) =>
            {
                policy.Add((i, j), 0);
                newPolicy.Add((i, j), 0);
            });

            bool constantReturnedCars = true;
            while (true)
            {
                console.WriteLine($"Policy: {iterations}");
                // policy evaluation (in-place)
                while (true)
                {
                    value.CopyTo(newValue);

                    ForEachLoop(MaxCars + 1, MaxCars + 1, (i, j) =>
                    {
                        newValue[i, j] = ExpectedReturn((i, j), policy[(i, j)], newValue, constantReturnedCars);
                    });

                    var valueChange = Matrix<double>.Abs(newValue - value).RowSums().Sum();
                    newValue.CopyTo(value);
                    if (valueChange < 0.0004)
                    {
                        //console.WriteLine(value.ToMatrixString());
                        break;
                    }
                }

                // policy improvement
                Copy(policy, newPolicy);
                ForEachLoop(MaxCars + 1, MaxCars + 1, (i, j) =>
                {
                    var actionReturns = Actions.Select( action =>
                    {
                        if ((action >= 0 && i >= action)
                            || (action < 0 && j >= Math.Abs(action)))
                        {
                            return ExpectedReturn((i, j), action, value, constantReturnedCars);
                        }
                        else
                        {
                            return (-double.MaxValue);
                        }
                    }).ToList();

                    var maxReturn = actionReturns.Max(x => x);
                    var indexOfAct = actionReturns.IndexOf(maxReturn);
                    newPolicy[(i, j)] = Actions[indexOfAct];
                });
                
                var policyChange = Diff(policy, newPolicy);

                Copy(newPolicy, policy);

                if (policyChange == 0)
                {
                    foreach (var item in policy)
                    {
                        console.WriteLine($"{item.Key} = {item.Value}, rewards = {value[item.Key.Item1, item.Key.Item2]}");
                    }
                    break;
                }

                ++iterations;
            }
        }
    }
}
