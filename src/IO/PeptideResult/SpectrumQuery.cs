using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace PhosphoRS.IO.PeptideResult
{
    public class SpectrumQuery
    {
        public string Spectrum { get; set; }

        public int Charge { get; set; }

        public IList<SearchHit> SearchResults { get; set; } = new List<SearchHit>();
    }
}