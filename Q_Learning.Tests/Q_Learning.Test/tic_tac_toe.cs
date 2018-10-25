using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Q_Learning.Test
{
    // https://github.com/ShangtongZhang/reinforcement-learning-an-introduction/blob/master/chapter01/tic_tac_toe.py
    public class tic_tac_toe
    {
        ITestOutputHelper console;
        public tic_tac_toe(ITestOutputHelper console)
        {
            this.console = console;
        }

        [Fact]
        public void Test1()
        {
            var state = new State();
            state.Data[0, 0] = 1;
            state.Data[1, 1] = 1;
            state.Data[2, 2] = 1;
            
            console.WriteLine($"{state.IsDone()}");

            var allStates = State.GetAllStates();
            //foreach (var (s, done) in allStates.Values.Take(10))
            //{
            //    s.Print(console);
            //}
            //state.Print(console);

            foreach (var i in Enumerable.Range(0, 10).Reverse())
            {
                console.WriteLine($"{i}");
            }
        }

        Player player1 = new Player(epsilon: 0.1);
        Player player2 = new Player(epsilon: 0.1);
        State currentState = new State();
        bool isEnd = false;
        [Fact]
        public void Test2()
        {
            play();
        }

        public int play(bool isPrint = true)
        {
            currentState = new State();
            player1.Reset();
            player2.Reset();
            isEnd = false;

            player1.SetSymbol(1);
            player2.SetSymbol(-1);

            player1.SetState(currentState);
            player2.SetState(currentState);

            foreach (var player in Alternate())
            {
                (int x, int y, double symbol) act = player.Act();
                var nextStateHash = currentState.NextState(act.x, act.y, act.symbol).Hash;

                (currentState, isEnd) = State.AllStates[nextStateHash];

                player1.SetState(currentState);
                player2.SetState(currentState);

                if (isPrint)
                {
                    console.WriteLine($"Player is {player.Symbol} act ({act.x}, {act.y}) apply policy {player.CurrentPolicy}");
                    currentState.Print(console);
                }

                if (isEnd)
                {
                    if (isPrint)
                    {
                        console.WriteLine($"The winner is {currentState.Winner}");
                        currentState.Print(console);
                    }
                    break;
                }

            }

            return currentState.Winner;
        }

        [Fact]
        public void train()
        {
            player1 = new Player(epsilon: 0);
            player2 = new Player(epsilon: 0);
            int player1_win = 0;
            int player2_win = 0;
            int episode = 10000;
            for (int i = 0; i < episode; i++)
            {
                int winner = play(false);
                if(winner == 1)
                {
                    ++player1_win;
                }
                if (winner == -1)
                {
                    ++player2_win;
                }

                //player1.Backup();
                //player2.Backup();
            }

            console.WriteLine($" player1_win = {player1_win}, player2_win = {player2_win}");
        }

        public IEnumerable<Player> Alternate()
        {
            while (true)
            {
                yield return player1;
                yield return player2;
            }
        }
    }
    
}
