using System;
using System.Collections.Generic;
using IMP.PhosphoRS;

namespace PhosphoRS.Wrapper
{
    public static class SpectrumTypeHelper
    {
        public static SpectrumType ParseSpectrumType(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(nameof(s));
            if (s.Equals("CID", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.CID_CAD;
            else if (s.Equals("CAD", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.CID_CAD;
            else if (s.Equals("CID_CAD", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.CID_CAD;
            else if (s.Equals("HCD", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.HCD;
            else if (s.Equals("ECD", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.ECD_ETD;
            else if (s.Equals("ETD", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.ECD_ETD;
            else if (s.Equals("ECD_ETD", StringComparison.InvariantCultureIgnoreCase))
                return SpectrumType.ECD_ETD;
            else
                return SpectrumType.None;
        }
    }
}