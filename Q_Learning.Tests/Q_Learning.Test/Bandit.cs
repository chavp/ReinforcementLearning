using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Q_Learning.Test
{
    public class Bandit
    {
        int KArm;
        public double Epsilon { get; set; }
        public double Initial { get; set; }
        public double StepSize { get; set; }
        bool SampleAverages = false;
        int? UcbParam = null;
        bool Gradient = false;
        bool GradientBaseline = false;
        double TrueReward;
        int Time;
        List<int> Indices;
        double AverageReward;

        Random Ran;
        public Bandit(
            int kArm = 4,
            double epsilon = 0.0,
            double initial = 0.0,
            double stepSize = 0.1,
            bool sampleAverages = false,
            int? ucbParam = null,
            bool gradient = false,
            bool gradientBaseline = false,
            double trueReward = 0.0)
        {
            KArm = kArm;
            Epsilon = epsilon;
            Initial = initial;
            StepSize = stepSize;
            SampleAverages = sampleAverages;
            UcbParam = ucbParam;
            Gradient = gradient;
            GradientBaseline = gradientBaseline;
            TrueReward = trueReward;

            Time = 0;

            Indices = Enumerable.Range(0, KArm).ToList();
            AverageReward = 0.0;

            Ran = new Random(DateTime.Now.Second);
            ProbabilityRandom = new ProbabilityRandom<int>();
        }

        List<double> QTrue;
        List<double> QEstimation;
        List<int> ActionCount;
        public int BestAction { get; protected set; }
        public void Reset()
        {
            QTrue = Enumerable.Range(0, KArm).Select(x => Ran.NextDouble() + TrueReward).ToList();
            QEstimation = Enumerable.Range(0, KArm).Select(x => Initial).ToList();
            ActionCount = Enumerable.Range(0, KArm).Select(x => 0).ToList();
            BestAction = QTrue.IndexOf(QTrue.Max());
        }

        List<double> UcbEstimation;
        List<double> ExpEst;
        List<double> ActionProb;
        double QBest;
        ProbabilityRandom<int> ProbabilityRandom;
        public (int, bool) Act()
        {
            if (Ran.NextDouble() < Epsilon)
            {
                return (Indices[Ran.Next(KArm)], false);
            }

            if (UcbParam.HasValue)
            {
                UcbEstimation = QEstimation.Select((data, index) =>
                {
                    return data + UcbParam.Value * Math.Sqrt(Math.Log(Time + 1) / (ActionCount[index] + (10 ^ -5)));
                }).ToList();

                QBest = UcbEstimation.Max();
                var ucbEstMaxList = UcbEstimation.Where(v => v == QBest).Select((data, index) => new {
                    Index = index,
                    Data = data
                }).ToList();
                return (ucbEstMaxList[Ran.Next(ucbEstMaxList.Count())].Index, true);
            }

            if (Gradient)
            {
                ExpEst = QEstimation.Select(qEst => Math.Exp(qEst)).ToList();
                ActionProb = ExpEst.Select(expEst => expEst / ExpEst.Sum()).ToList();
                ProbabilityRandom.Reset();
                for (int i = 0; i < ActionProb.Count; i++)
                {
                    ProbabilityRandom.SetProb(i, ActionProb[i]);
                }
                //var acts = ActionProb.TakeWhile(p => Ran.NextDouble() < p).Select((d, i) => i).ToList();
                //if (acts.Count == 0)
                //    return (ActionProb.Select((d, i) => i).ToList()[Ran.Next(ActionProb.Count)], false);
                //return (acts[Ran.Next(acts.Count)], false);
                return (ProbabilityRandom.Next(), false);
            }

            return (QEstimation.IndexOf(QEstimation.Max()), true);
        }

        List<double> OneHot;
        double Baseline;
        Dictionary<int, double> rewardDic = new Dictionary<int, double>
        {
            { 0, 1},
            { 1, 1},
            { 2, 2},
            { 3, 2},
            { 4, 0},
        };

        public double Step(int action)
        {
            var reward = Ran.NextDouble() + QTrue[action];
            //var reward = rewardDic[action];
            Time += 1;
            AverageReward = (Time - 1) / (Time * AverageReward) + reward / Time;
            ActionCount[action] += 1;

            if (SampleAverages)
            {
                QEstimation[action] += 1.0 / ActionCount[action] * (reward - QEstimation[action]);
            }
            else if (Gradient)
            {
                OneHot = Enumerable.Range(0, KArm).Select(x => 0.0).ToList();
                OneHot[action] = 1;
                if (GradientBaseline)
                {
                    Baseline = AverageReward;
                }
                else
                {
                    Baseline = 0;
                }

                QEstimation = QEstimation.Select((d, i) => d + StepSize + (reward - Baseline) * (OneHot[i] - ActionProb[i])).ToList();
            }
            else
            {
                QEstimation[action] += StepSize * (reward - QEstimation[action]);
            }
            return reward;
        }
    }
}
