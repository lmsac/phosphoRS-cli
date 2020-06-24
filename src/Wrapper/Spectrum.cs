using System.Collections.Generic;
using IMP.PhosphoRS;

namespace PhosphoRS.Wrapper
{
    public class Spectrum
    {
        public Spectrum(int id, string name, int precursorCharge, SpectrumType activationType, IList<Peak> peaks)
        {
            ID = id;
            Name = name;
            PrecursorCharge = precursorCharge;
            ActivationType = activationType;
            Peaks = peaks;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public int PrecursorCharge { get; set; }

        public SpectrumType ActivationType { get; set; }

        public IList<Peak> Peaks { get; set; }

        public IList<Peptide> IdentifiedPhosphorPeptides { get; set; } = new List<Peptide>();
    }
}