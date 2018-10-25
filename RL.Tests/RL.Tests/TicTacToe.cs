using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace RL.Tests
{
    // https://github.com/ShangtongZhang/reinforcement-learning-an-introduction/blob/master/chapter01/tic_tac_toe.py
    public class TicTacToe
    {
        private readonly ITestOutputHelper output;

        public TicTacToe(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void test1()
        {
            var state = new State();
            state.Data[0, 0] = 1;
            state.Data[1, 1] = 1;
            state.Data[2, 2] = 1;
            output.WriteLine($"{state.IsEnd()}");
            state.Print(output);

            Assert.Equal(1, state.Winner);

            state = new State();
            state.Data[0, 0] = -1;
            state.Data[1, 1] = -1;
            state.Data[2, 2] = -1;
            state.IsEnd();
            Assert.Equal(-1, state.Winner);

            var all_states = State.GetAllStates();

            var win1 = all_states.Where(x => x.Value.Item1.Winner == 1);
            var win2 = all_states.Where(x => x.Value.Item1.Winner == -1);

            var draw = all_states.Where(x => x.Value.Item1.Winner == 0);

            foreach (var item in draw)
            {
                item.Value.Item1.Print(output);
            }
        }
    }

    public class State
    {
        static int BOARD_ROWS = 3;
        static int BOARD_COLS = 3;
        static int BOARD_SIZE = 0;

        int? winner;
        public int? Winner
        {
            get { return winner; }
        }

        double? hash_val;
        bool? isEnd;

        Matrix<double> data;
        public Matrix<double> Data
        {
            get { return data; }
            set
            {
                data = value;
            }
        }
        public State()
        {
            BOARD_SIZE = BOARD_ROWS * BOARD_COLS;

            data = Matrix<double>.Build.Dense(BOARD_ROWS, BOARD_COLS);

        }

        public double Hash()
        {
            if (!this.hash_val.HasValue)
            {
                this.hash_val = 0;
                for (int i = 0; i < this.data.RowCount; i++)
                {
                    for (int j = 0; j < this.data.ColumnCount; j++)
                    {
                        var k = this.data[i, j];
                        if (k == -1)
                            k = 2;

                        this.hash_val = this.hash_val * 3 + k;
                    }
                }
            }
            return this.hash_val.GetValueOrDefault();
        }

        public bool IsEnd()
        {
            if (this.isEnd.HasValue)
                return this.isEnd.Value;

            var results = new List<double>();
            results.AddRange(this.data.RowSums());
            results.AddRange(this.data.ColumnSums());
            double sum1 = 0, sum2 = 0;
            for (int i = 0; i < this.data.RowCount; i++)
            {
                sum1 += this.data[i, i];
                sum2 += this.data[i, BOARD_COLS - 1 - i];
            }
            results.Add(sum1);
            results.Add(sum2);

            foreach (var result in results)
            {
                if(result == 3)
                {
                    this.winner = 1;
                    this.isEnd = true;
                    return this.isEnd.GetValueOrDefault();
                }
                else if(result == -3)
                {
                    this.winner = -1;
                    this.isEnd = true;
                    return this.isEnd.GetValueOrDefault();
                }
            }

            var sum = Matrix<double>.Abs(this.data).RowSums().Sum();
            if(sum == BOARD_COLS * BOARD_ROWS)
            {
                this.winner = 0;
                this.isEnd = true;
                return this.isEnd.GetValueOrDefault();
            }

            this.isEnd = false;
            return this.isEnd.GetValueOrDefault();
        }

        public State NextState(int i, int j, int symbol)
        {
            var newState = new State();
            this.data.CopyTo(newState.Data);
            newState.Data[i, j] = symbol;
            return newState;
        }

        public void Print(ITestOutputHelper output)
        {
            output.WriteLine(Data.ToMatrixString());
        }

        public static void GetAllStatesImpl(State current_state, int current_symbol
            , Dictionary<double,(State, bool)> all_states)
        {
            for (int i = 0; i < BOARD_ROWS; i++)
            {
                for (int j = 0; j < BOARD_COLS; j++)
                {
                    if(current_state.Data[i, j] == 0)
                    {
                        var newState = current_state.NextState(i, j, current_symbol);
                        var newHash = newState.Hash();
                        if (!all_states.ContainsKey(newHash))
                        {
                            var isEnd = newState.IsEnd();
                            all_states.Add(newHash, (newState, isEnd));
                            if (!isEnd)
                                GetAllStatesImpl(newState, -current_symbol, all_states);
                        }
                    }
                }
            }
        }

        public static Dictionary<double, (State, bool)> GetAllStates()
        {
            var current_symbol = 1;
            var current_state = new State();
            var all_states = new Dictionary<double, (State, bool)>();
            all_states[current_state.Hash()] = (current_state, current_state.IsEnd());
            GetAllStatesImpl(current_state, current_symbol, all_states);
            return all_states;
        }
    }

    public class Judger
    {

    }

    public class Player
    {

    }
}
