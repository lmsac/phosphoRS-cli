using System.Collections.Generic;

namespace PhosphoRS.Wrapper
{
    public class Peptide
    {
        public Peptide(int id, string sequence, string modification)
        {
            ID = id;
            Sequence = sequence;
            Modification = modification;
        }

        public int ID { get; set; }

        public string Sequence { get; set; }

        public string Modification { get; set; }
    }
}