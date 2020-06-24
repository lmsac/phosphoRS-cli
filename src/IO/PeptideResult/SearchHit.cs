using System;
using System.Collections.Generic;

namespace PhosphoRS.IO.PeptideResult
{
    public class SearchHit
    {
        public string Peptide { get; set; }

        public int Rank { get; set; }

        public IList<Modification> Modifications { get; set; } = new List<Modification>();

        public IDictionary<string, double> SearchScores { get; set; } = new Dictionary<string, double>();
    }
}