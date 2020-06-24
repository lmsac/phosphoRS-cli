using System.Collections.Generic;

namespace PhosphoRS.Wrapper
{
    public class PeptideResult
    {
        public PeptideResult(int id, string sequence)
        {
            ID = id;
            Sequence = sequence;
        }

        public int ID { get; set; }

        public string Sequence { get; set; }

        public IList<SitePrediction> SitePrediction { get; set; } = new List<SitePrediction>();

        public IList<IsoformPrediction> Isoforms { get; set; } = new List<IsoformPrediction>();
    }
}