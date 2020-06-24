using System.Collections.Generic;

namespace PhosphoRS.Wrapper
{
    public class PhosphoRSInput
    {
        public PhosphoRSOptions Options { get; set; } = new PhosphoRSOptions();

        public IList<Modification> Modifications { get; set; } = new List<Modification>();

        public IList<Spectrum> Spectra { get; set; } = new List<Spectrum>();
    }
}