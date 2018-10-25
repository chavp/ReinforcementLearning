using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public class Maze
    {
        public int WORLD_WIDTH = 9;
        public int WORLD_HEIGHT = 6;

        static readonly int ACTION_UP = 0;
        static readonly int ACTION_DOWN = 1;
        static readonly int ACTION_LEFT = 2;
        static readonly int ACTION_RIGHT = 3;

        public static readonly List<int> actions = new List<int> {
            ACTION_UP,
            ACTION_DOWN,
            ACTION_LEFT,
            ACTION_RIGHT
        };

        public (int, int) START_STATE = (2, 0);
        public List<(int, int)> GOAL_STATES = new List<(int, int)> { (0, 8) };

        public List<(int, int)> obstacles = new List<(int, int)>
        {
            (1, 2),
            (2, 2),
            (3, 2),
            (0, 7),
            (1, 7),
            (2, 7),
            (4, 2),
        };
        static List<(int, int)> old_obstacles = new List<(int, int)>();
        static List<(int, int)> new_obstacles = new List<(int, int)>();

        static List<(int, int)> obstacle_switch_time = new List<(int, int)>();

        public (int, int, int) q_size;

        public static readonly int max_steps = int.MaxValue;
        public int resolution = 1;

        public Maze()
        {
            q_size = (WORLD_HEIGHT, WORLD_WIDTH, actions.Count);
        }

        public List<(int, int)> extend_state((int, int) state, int factor)
        {
            var new_state = (state.Item1 * factor, state.Item2 * factor);
            var new_states = new List<(int, int)>();
            for (int i = 0; i < factor; i++)
            {
                for (int j = 0; j < factor; j++)
                {
                    new_states.Add((new_state.Item1 + i, new_state.Item2 + j));
                }
            }
            return new_states;
        }

        public Maze extend_maze(int factor)
        {
            var new_maze = new Maze();
            new_maze.WORLD_WIDTH = this.WORLD_WIDTH * factor;
            new_maze.WORLD_HEIGHT = this.WORLD_HEIGHT * factor;
            new_maze.START_STATE = (this.START_STATE.Item1 * factor, this.START_STATE.Item2 * factor);
            new_maze.GOAL_STATES = this.extend_state(this.GOAL_STATES.First(), factor);
            new_maze.obstacles = new List<(int, int)>();
            foreach (var state in this.obstacles)
            {
                new_maze.obstacles.AddRange(this.extend_state(state, factor));
            }
            new_maze.q_size = (new_maze.WORLD_HEIGHT, new_maze.WORLD_WIDTH, Maze.actions.Count);
            new_maze.resolution = factor;
            return new_maze;
        }

        public ((int, int), double) step((int, int) state, int action)
        {
            double reward = 0;
            (int x, int y) = state;
            if(action == ACTION_UP)
            {
                x = Math.Max(x - 1, 0);
            }
            else if (action == ACTION_DOWN)
            {
                x = Math.Min(x + 1, this.WORLD_HEIGHT - 1);
            }
            else if (action == ACTION_LEFT)
            {
                y = Math.Max(y - 1, 0);
            }
            else if (action == ACTION_RIGHT)
            {
                y = Math.Min(y + 1, this.WORLD_WIDTH - 1);
            }

            var countO = this.obstacles.Where(o => o.Item1 == x && o.Item2 == y).Count();
            if (countO > 0)
                (x, y) = state;

            var countG = this.GOAL_STATES.Where( o => o.Item1 == x && o.Item2 == y).Count();
            if (countG > 0)
                reward = 1;
            else
                reward = 0;
            return ( (x, y), reward );
        }

    }
}
