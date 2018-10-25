using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Q_Learning.Test
{
    public class ProbabilityRandom<K>
    {
        public Dictionary<K, double> DicOfProbs { get; set; }
        public ProbabilityRandom()
        {
            DicOfProbs = new Dictionary<K, double>();
        }

        public void Reset()
        {
            DicOfProbs.Clear();
        }

        public void SetProb(K key, double prob)
        {
            if (prob < 0) prob = 0;
            if (prob > 1) prob = 1;
            var sumProb = Math.Round(DicOfProbs.Values.Sum());
            if (sumProb > 1)
                throw new ArgumentOutOfRangeException("Sum probability > 1");

            if (!DicOfProbs.ContainsKey(key))
            {
                DicOfProbs.Add(key, prob);
            }

            DicOfProbs[key] = prob;

            ListOfKeyNumber.RemoveAll(x => x.Equals(key));

            for (int i = 0; i < Math.Round(prob * 100); i++)
            {
                ListOfKeyNumber.Add(key);
            }

            ListOfKeyNumber = ListOfKeyNumber
                .OrderBy(x => Guid.NewGuid())
                .ToList();
        }

        Random rand = new Random();
        List<K> ListOfKeyNumber = new List<K>();
        public K Next()
        {
            return ListOfKeyNumber[rand.Next(0, ListOfKeyNumber.Count)];
        }
    }
}