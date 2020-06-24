using System.Collections.Generic;
using IMP.PhosphoRS;

namespace PhosphoRS.IO.Spectral
{
    public class Spectrum
    {
        public string Title 
        { 
            get 
            {
                if (_title == null)
                    Fields.TryGetValue("TITLE", out _title); 
                return _title;
            }
            set 
            {
                if (!Fields.TryAdd("TITLE", value))
                    Fields["TITLE"] = value;
                _title = value;
            } 
        }

        public IList<Peak> Peaks { get; set; } = new List<Peak>();

        public IDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();

        private string _title;

    }
}