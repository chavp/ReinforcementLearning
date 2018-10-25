using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test
{
    public class Player
    {
        Dictionary<double, double> Estimations;
        double StepSize;
        double Epsilon;
        List<State> States;
        List<bool> Greedy;
        List<double> Actions;
        public double Symbol { get; private set; }
        public string CurrentPolicy { get; private set; }
        Random random;
        public Player(double stepSize = 0.1, double epsilon = 0.1)
        {
            Estimations = new Dictionary<double, double>();
            foreach (var (hash, (state, done)) in State.AllStates)
            {
                Estimations.Add(hash, 0);
            }

            StepSize = stepSize;
            Epsilon = epsilon;

            Reset();
        }

        public void Reset()
        {
            States = new List<State>();
            Greedy = new List<bool>();
            Actions = new List<double>();

            random = new Random(DateTime.Now.Second);
        }

        public void SetState(State state)
        {
            States.Add(state);
            Greedy.Add(true);
        }

        public void SetSymbol(double symbol)
        {
            Symbol = symbol;
            foreach (var (hash, (state, done)) in State.AllStates)
            {
                if (done)
                {
                    if (state.Winner == symbol)
                    {
                        Estimations[hash] = 1;
                    }
                    else if (state.Winner == 0)
                    {
                        Estimations[hash] = 0.5;
                    }
                }
            }

            var max = Estimations.Where(x => x.Value == 1).ToList();
        }

        public void Backup()
        {
            var states = States.Select(x => x.Hash).ToList();
            for (int i = states.Count() - 2; i > 0; i--)
            {
                var state = states[i];
                var state_ = states[i + 1];
                if (Greedy[i])
                {
                    var tdError = (Estimations[state_] - Estimations[state]);
                    Estimations[state] += StepSize * tdError;
                }
            }
        }

        public (int, int, double) Act()
        {
            var state = States.Last();
            var nextStatePos = new Dictionary<double, (int, int)>();
            var listOfHash = new List<double>();
            for (int i = 0; i < State.BOARD_ROWS; i++)
            {
                for (int j = 0; j < State.BOARD_COLS; j++)
                {
                    if (state.Data[i, j] == 0)
                    {
                        nextStatePos.Add(state.NextState(i, j, Symbol).Hash, (i, j));
                        listOfHash.Add(state.NextState(i, j, Symbol).Hash);
                    }
                }
            }

            CurrentPolicy = "non-greedy";
            (int x, int y) actPos;
            if (random.NextDouble() < Epsilon)
            {
                actPos = nextStatePos[listOfHash[random.Next(0, listOfHash.Count)]];
                Greedy[Greedy.Count - 1] = false;
                return (actPos.x, actPos.y, Symbol);
            }

            CurrentPolicy = "greedy";
            var values = new List<(double, (int, int))>();
            foreach (var (hash, pos) in nextStatePos)
            {
                values.Add((Estimations[hash], pos));
            }

            if(values.Where(x => x.Item1 == 1.0).Count() > 0)
            {
                var est = values.Where(x => x.Item1 == 1.0).ToList();
            }

            values = values.OrderBy(a => Guid.NewGuid()).ToList();
            values = values.OrderByDescending(a => a.Item1).ToList();
            actPos = values.First().Item2;
            return (actPos.x, actPos.y, Symbol);
        }
    }
}
