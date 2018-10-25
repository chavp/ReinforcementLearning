using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    public class State
    {
        public readonly static int BOARD_ROWS = 3;
        public readonly static int BOARD_COLS = 3;
        public readonly static int BOARD_SIZE;
        static State()
        {
            BOARD_SIZE = BOARD_ROWS * BOARD_COLS;
            AllStates = GetAllStates();
        }

        public Matrix<double> Data { get; set; }
        public int Winner { get; private set; }
        double? hash = null;
        bool? isDone = null;

        public State()
        {
            Data = Matrix<double>.Build.Dense(BOARD_ROWS, BOARD_COLS);
        }

        public double Hash
        {
            get
            {
                string hashString = "";
                if (!hash.HasValue)
                {
                    hash = 0;
                    for (int i = 0; i < BOARD_ROWS; i++)
                    {
                        for (int j = 0; j < BOARD_COLS; j++)
                        {
                            hashString += $"|{Data[i, j]}|";
                        }
                    }
                }
                return hashString.GetHashCode();
            }
        }

        public bool IsDone()
        {
            if (isDone.HasValue) return isDone.Value;

            var results = new List<double>();
            results.AddRange(Data.RowSums());
            results.AddRange(Data.ColumnSums());
            results.Add(Data.Diagonal().Sum());
            double sum = 0;
            for (int i = 0; i < BOARD_ROWS; i++)
            {
                sum += Data[0, BOARD_ROWS - 1 - i];
            }
            results.Add(sum);

            foreach (var result in results)
            {
                if (result == 3)
                {
                    Winner = 1;
                    isDone = true;
                    return isDone.Value;
                }
                else if (result == -3)
                {
                    Winner = -1;
                    isDone = true;
                    return isDone.Value;
                }
            }

            sum = Data.RowAbsoluteSums().Sum();
            if (sum == BOARD_SIZE)
            {
                Winner = 0;
                isDone = true;
                return isDone.Value;
            }
            isDone = false;
            return isDone.Value;
        }

        public State NextState(int i, int j, double symbol)
        {
            var newState = new State();
            Data.CopyTo(newState.Data);
            newState.Data[i, j] = symbol;
            return newState;
        }

        public void Print(ITestOutputHelper console)
        {
            console.WriteLine("- - - - -");
            for (int i = 0; i < BOARD_ROWS; i++)
            {
                console.WriteLine($"| {string.Join(" ", Data.Row(i))} |");
            }
            console.WriteLine("- - - - -");
        }

        private static Dictionary<double, (State, bool)> getAllStatesImpl
            (State currentState, double currentSymbol,
            Dictionary<double, (State, bool)> allStates)
        {
            for (int i = 0; i < BOARD_ROWS; i++)
            {
                for (int j = 0; j < BOARD_COLS; j++)
                {
                    if (currentState.Data[i, j] == 0)
                    {
                        var newState = currentState.NextState(i, j, currentSymbol);
                        var newHash = newState.Hash;
                        if (!allStates.ContainsKey(newHash))
                        {
                            var isDone = newState.IsDone();
                            allStates.Add(newHash, (newState, isDone));
                            if (!isDone) getAllStatesImpl(newState, -currentSymbol, allStates);
                        }
                    }
                }
            }
            return allStates;
        }

        public static Dictionary<double, (State, bool)> GetAllStates()
        {
            var currentSymbol = 1;
            var currentState = new State();
            var allStates = new Dictionary<double, (State, bool)>();
            getAllStatesImpl(currentState, currentSymbol, allStates);
            return allStates;
        }

        public static Dictionary<double, (State, bool)> AllStates { get; private set; }
    }

}
