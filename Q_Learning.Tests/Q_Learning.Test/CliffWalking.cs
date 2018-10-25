using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Q_Learning.Test
{
    public class CliffWalking
    {
        int Hight;
        int Width;
        List<string> ActionSpace;
        (int, int) CurrentPosition;
        Dictionary<(int, int), string> Env;
        //Dictionary<(int, int), int> StateMapper;
        // self.env = np.array(range(self.observation_space), dtype='U16').reshape(self.hight, self.width)

        QAgent qAgent;
        int CurrentState;
        Random Random;
        List<double> R;
        Matrix<double> StateMapper = null;

        public CliffWalking(int hight, int width)
        {
            Random = new Random(DateTime.Now.Second);
            Hight = hight;
            Width = width;
            ActionSpace = new List<string>
            {
                "A", "V", "<", ">"
            };
            R = new List<double>
            {
                -1, -100, 100
            };

            StateMapper = Matrix<double>.Build.Dense(Hight, Width);

            Env = new Dictionary<(int, int), string>();
            //StateMapper = new Dictionary<(int, int), int>();
            int stateCounter = 0;
            for (int h = 0; h < Hight; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    Env.Add((h, w), " ");
                    StateMapper[h, w] = stateCounter++;
                }
            }

            
            for (int w = 1; w < Width-1; w++)
            {
                for (int i = 0; i < Hight * 0.3; i++)
                {
                    var h = Random.Next(0, Hight);
                    Env[(h, w)] = "X";
                }
                
            }
            
            Env[(0, Width - 1)] = "G";

            qAgent = new QAgent(ObservationSpace, ActionSpaceSize);
            CurrentState = Reset();
        }

        public (int, int, string) ActionSpaceSample()
        {
            (int act, string info) choAct = qAgent.ChooseAction(CurrentState);
            return (CurrentState, choAct.act, choAct.info);
        }

        public void ReduceExploration(double i)
        {
            qAgent.ReduceExploration(i);
        }

        public (int, double, bool, string) Step(int action)
        {
            int nextState = 0;
            double reward = R[0];
            bool done = false;
            string info = string.Empty;

            var nextPosition = CurrentPosition;
            Env[CurrentPosition] = " ";
            string act = ActionSpace[action];
            // "U", "D", "L", "R"
            if (ActionSpace[action] == ActionSpace[0] && CurrentPosition.Item1 > 0)
            {
                nextPosition = (CurrentPosition.Item1 - 1, CurrentPosition.Item2);
            }
            else if (ActionSpace[action] == ActionSpace[1] && CurrentPosition.Item1 < Hight - 1)
            {
                nextPosition = (CurrentPosition.Item1 + 1, CurrentPosition.Item2);
            }
            else if (ActionSpace[action] == ActionSpace[2] && CurrentPosition.Item2 > 0)
            {
                nextPosition = (CurrentPosition.Item1, CurrentPosition.Item2 - 1);
            }
            else if (ActionSpace[action] == ActionSpace[3] && CurrentPosition.Item2 < Width - 1)
            {
                nextPosition = (CurrentPosition.Item1, CurrentPosition.Item2 + 1);
            }

            nextState = (int)StateMapper[nextPosition.Item1, nextPosition.Item2];

            if (Env[nextPosition] == "X")
            {
                reward = R[1];
            }

            if (Env[nextPosition] == "G")
            {
                reward = R[2];
            }

            if (Env[nextPosition] == "X" || Env[nextPosition] == "G")
            {
                done = true;
                Env[CurrentPosition] = act;
            }

            if(!done 
                && Env[nextPosition] != "X" 
                && Env[nextPosition] != "G")
                Env[nextPosition] = act;
            
            qAgent.Learn(CurrentState, action, nextState, reward, done);
            CurrentState = nextState;
            CurrentPosition = nextPosition;

            return (nextState, reward, done, info);
        }

        public int Reset()
        {
            for (int h = 0; h < Hight; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    if (Env[(h, w)] != "X" && Env[(h, w)] != "G")
                    {
                        Env[(h, w)] = " ";
                    }
                }
            }

            Env[(Hight - 1, 0)] = "*";
            CurrentPosition = (Hight - 1, 0);
            CurrentState = (int)StateMapper[CurrentPosition.Item1, CurrentPosition.Item2];
            return CurrentState;
        }

        public void RenderEnv(ITestOutputHelper console = null)
        {
            for (int h = 0; h < Hight; h++)
            {
                var rows = from x in Env
                           where x.Key.Item1 == h
                           select x.Value;

                var msg = $"|{string.Join(",", rows)}|";
                if (console != null)
                    console.WriteLine(msg);
                else
                    System.Console.WriteLine(msg);
            }
        }

        public void RenderQ(ITestOutputHelper console = null)
        {
            var msg = $"(S) = [ {string.Join(",", ActionSpace)} ] ";
            if (console != null)
                console.WriteLine(msg);
            else
                System.Console.WriteLine(msg);

            qAgent.RenderQ(console);
        }

        public int ObservationSpace
        {
            get
            {
                return Hight * Width;
            }
        }

        public string MapAction(int action)
        {
            return ActionSpace[action];
        }

        public int ActionSpaceSize
        {
            get
            {
                return ActionSpace.Count;
            }
        }
    }
}
