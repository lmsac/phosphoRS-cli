using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhosphoRS.IO.Spectral
{
    public class MgfReader : ISpectraReader
    {
        private readonly TextReader _reader;

        public MgfReader(Stream stream)
            : this(new StreamReader(stream))
        {            
        }

        public MgfReader(TextReader reader)
        {
            _reader = reader;
        }

        public MgfReader(string path)
            : this(File.OpenText(path))
        {
        }

        public virtual void Close() => _reader.Close();

        public void Dispose() => _reader.Dispose();

        public Spectrum Read()
        {
            Spectrum spectrum = null;

            while (true)
            {
                var line = _reader.ReadLine();
                if (line == null)
                    break;

                if (line.TrimEnd().Equals("BEGIN IONS"))
                    spectrum = new Spectrum();
                
                var matchField = Regex.Match(line, "^([A-Za-z0-9_\\.\\s]+)=(.*)$");
                if (matchField.Groups.Count == 3)
                {
                    spectrum?.Fields?.TryAdd(
                        key: matchField.Groups[1].Value.Trim(),
                        value: matchField.Groups[2].Value.Trim()
                    );
                }

                var matchPeak = Regex.Match(line, "^([0-9]*(?:\\.[0-9]*)?)[\\s]+([0-9]*(?:\\.[0-9]*)?)");
                if (matchPeak.Groups.Count == 3)
                {
                    spectrum?.Peaks?.Add(new IMP.PhosphoRS.Peak(
                        massZ: double.Parse(matchPeak.Groups[1].Value),
                        intensity: double.Parse(matchPeak.Groups[2].Value)
                    ));
                }

                if (line.TrimEnd().Equals("END IONS"))
                    break;
            }

            return spectrum;
        }    
    }
}