using System.Collections.Generic;

namespace PhosphoRS.Wrapper
{
    public class IsoformPrediction
    {
        public IsoformPrediction(int id, IList<int> sites, double score, double probability)
        {
            ID = id;
            Sites = sites;
            Score = score;
            Probability = probability;
        }

        public int ID { get; set; }

        public IList<int> Sites { get; set; }

        public double Score { get; set; }

        public double Probability { get; set; }
    }
}