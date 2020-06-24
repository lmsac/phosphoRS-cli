using System.Collections.Generic;

namespace PhosphoRS.Wrapper
{
    public class PhosphoRSOutput
    {
        public PhosphoRSOptions Options { get; set; } = new PhosphoRSOptions();

        public IList<Modification> Modifications { get; set; } = new List<Modification>();

        public IList<SpectrumResult> Spectra { get; set; } = new List<SpectrumResult>();
    }
}