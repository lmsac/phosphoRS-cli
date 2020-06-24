using System;
using System.Collections.Generic;
using System.Linq;
using PhosphoRS.IO.PeptideResult;
using PhosphoRS.IO.Spectral;
using PhosphoRS.Wrapper;
using PhosphoRS.Util;

namespace PhosphoRS.IO
{
    public class PeptideResultSpectraInputReader : IPhosphoRSInputReader
    {
        private readonly IPeptideResultReader _peptideReader;

        private readonly ISpectraReader _spectralReader;

        private readonly PhosphoRSOptions _options;

        public PeptideResultSpectraInputReader(
            IPeptideResultReader peptideReader,
            ISpectraReader spectralReader,
            PhosphoRSOptions options = null
        )
        {
            if (peptideReader == null)
                throw new ArgumentNullException(nameof(peptideReader));
            if (spectralReader == null)
                throw new ArgumentNullException(nameof(spectralReader));
            _peptideReader = peptideReader;
            _spectralReader = spectralReader;
            _options = options;
        }

        public virtual void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _peptideReader.Dispose();
            _spectralReader.Dispose();
        }

        public PhosphoRSInput Read()
        {
            var spectra = new Dictionary<string, Spectral.Spectrum>();
            while (true)
            {
                var spectrum = _spectralReader.Read();
                if (spectrum == null)
                    break;
                spectra.TryAdd(
                    key: spectrum.Title,
                    value: spectrum
                );
            }

            var spectrumQueries = new List<SpectrumQuery>();
            while (true)
            {
                var spectrumQuery = _peptideReader.Read();
                if (spectrumQuery == null)
                    break;
                spectrumQueries.Add(spectrumQuery);
            }

            var input = new PhosphoRSInput(); 
            if (_options != null)
                input.Options = _options;                       
            input.Modifications.Add(Wrapper.Modification.PhosphoSTY);
            input.Modifications.Add(Wrapper.Modification.OxidationM);
            input.Modifications.Add(Wrapper.Modification.CarbamidomethylC);
            input.Options.PhosphorylationSymbol = input.Modifications[0].Symbol;
            
            spectrumQueries.ForEach((query, i) =>
            {
                if (query.SearchResults?.Count > 0)
                {
                    if (spectra.TryGetValue(query.Spectrum, out var spec))
                    {
                        var spectrum = new Wrapper.Spectrum(
                            id: (int)i,
                            name: query.Spectrum,
                            precursorCharge: query.Charge,
                            activationType: input.Options.ActivationType,
                            peaks: spec.Peaks
                        );

                        query.SearchResults.ForEach(hit =>
                        {
                            if (_options.MaxHitRank > 0 && hit.Rank > _options.MaxHitRank)
                                return;

                            var modification = Enumerable.Repeat('0', hit.Peptide.Length).ToArray();
                            hit.Modifications.ForEach(mod => 
                            {
                                double Mass(char aminoAcid)
                                {
                                    if (IMP.PhosphoRS.AminoAcid.FindAAResidue(aminoAcid, out var aa))
                                        return aa.MonoisotopicMass;
                                    return 0;
                                };

                                modification[mod.Position - 1] =
                                    input.Modifications
                                        ?.Where(m => m.TargetAminoAcids.Contains(hit.Peptide[mod.Position - 1]))
                                        ?.Where(m => Math.Abs(Mass(hit.Peptide[mod.Position - 1]) + m.MassDelta - mod.Mass) <= 0.5)
                                        ?.OrderBy(m => Math.Abs(Mass(hit.Peptide[mod.Position - 1]) + m.MassDelta - mod.Mass))
                                        ?.FirstOrDefault()
                                        ?.Symbol ?? '0';
                            });
                            var peptide = new Peptide(
                                id: (int)i + spectrumQueries.Count * (hit.Rank - 1),
                                sequence: hit.Peptide,
                                modification: "0." + string.Join("", modification) + ".0"
                            );

                            spectrum.IdentifiedPhosphorPeptides.Add(peptide);
                        });

                        input.Spectra.Add(spectrum);
                    }
                }
            });
            return input;
        }
    }
}