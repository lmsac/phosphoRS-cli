using System;
using System.IO;
using System.Linq;
using PhosphoRS.IO.Util;
using PhosphoRS.Util;
using PhosphoRS.Wrapper;

namespace PhosphoRS.IO
{
    public class PhosphoRSOutputCsvWriter : IPhosphoRSOutputWriter
    {
        private readonly TableWriter _writer;

        public PhosphoRSOutputCsvWriter(Stream stream)
            : this(new StreamWriter(stream))
        {
        }

        public PhosphoRSOutputCsvWriter(TextWriter writer)
            : this(new TableWriter(writer, ','))
        {
        }

        public PhosphoRSOutputCsvWriter(string path)
            : this(File.CreateText(path))
        {
        }

        public PhosphoRSOutputCsvWriter(TableWriter writer)
        {
            _writer = writer;
        }

        public virtual void Close() => _writer.Close();

        public void Dispose() => _writer.Dispose();

        public void Write(PhosphoRSOutput output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            
            _writer.WriteRow(
                "Spectrum.ID", 
                "Spectrum.Name", 
                "Spectrum.PrecursorCharge", 
                "Spectrum.ActivationType",
                "Peptide.ID",
                "Peptide.Sequence",
                "Peptide.SitePrediction",
                "Isoform.ID",
                "Isoform.Sites",
                "Isoform.Score",
                "Isoform.Probability"
            );

            string SitePredictionToString(Wrapper.PeptideResult peptide)
            {
                return string.Join(
                    " ", 
                    peptide.SitePrediction
                        .Select(site => $"{peptide.Sequence[site.Position - 1]}{site.Position}({site.Probability.ToString("0.###")})")
                );
            }
                
            output.Spectra.ForEach(spectrum =>
            {
                spectrum.Peptides.ForEach(peptide =>
                {
                    peptide.Isoforms.ForEach(isoform =>
                    {
                        _writer.WriteRow(new string[] {
                            spectrum.ID.ToString(),
                            spectrum.Name,
                            spectrum.PrecursorCharge.ToString(),
                            spectrum.ActivationType.ToString(),
                            peptide.ID.ToString(),
                            peptide.Sequence,
                            SitePredictionToString(peptide),
                            isoform.ID.ToString(),
                            string.Join(" ", isoform.Sites.Select(site => $"{peptide.Sequence[site - 1]}{site}")),
                            isoform.Score.ToString(),
                            isoform.Probability.ToString()
                        });
                    });
                });
            });
        }
    }
}