using System.Collections.Generic;
using IMP.PhosphoRS;

namespace PhosphoRS.Wrapper
{
    public class SpectrumResult
    {
        public SpectrumResult(int id, string name, int precursorCharge, SpectrumType activationType)
        {
            ID = id;
            Name = name;
            PrecursorCharge = precursorCharge;
            ActivationType = activationType;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public int PrecursorCharge { get; set; }

        public SpectrumType ActivationType { get; set; }

        public IList<PeptideResult> Peptides { get; set; } = new List<PeptideResult>();
    }
}