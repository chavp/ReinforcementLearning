using System;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test.chapter08
{
    public class PriorityQueue
    {
        Dictionary<((int, int), int), (int, int, ((int, int), int))> entry_finder = new Dictionary<((int, int), int), (int, int, ((int, int), int))>();
        ((int, int), int) REMOVED = ((-1, -1), -1);
        int counter = 0;
        Stack<(int, int, ((int, int), int))> pq = new Stack<(int, int, ((int, int), int))>();
        public void add_item(((int, int), int) item, int priority = 0)
        {
            if (entry_finder.ContainsKey(item))
            {
                remove_item(item);
            }
            var entry = (priority, counter, item);
            counter += 1;
            entry_finder.Add(item, entry);
            pq.Push(entry);
        }

        public void remove_item(((int, int), int) item)
        {
            var entry = entry_finder[item];
            entry_finder.Remove(item);
            entry = (entry.Item1, entry.Item2, REMOVED);
        }

        public (((int, int), int), int) pop_item()
        {
            while (pq.Count > 0)
            {
                (int priority, int count, ((int, int) state, int action)) = pq.Pop();
                if(!(state.Item1 == REMOVED.Item1.Item1 
                    && state.Item2 == REMOVED.Item1.Item2))
                {
                    entry_finder.Remove((state, action));
                    return ((state, action), priority);
                }
            }
            throw new ArgumentException("pop from an empty priority queue");
        }

        public bool empty()
        {
            return entry_finder.Count == 0;
        }
    }
}
