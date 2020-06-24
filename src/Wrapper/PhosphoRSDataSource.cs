using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IMP.PhosphoRS;
using PhosphoRS.Util;

namespace PhosphoRS.Wrapper
{
    public class PhosphoRSDataSource
    {
        public PhosphoRSDataSource(PhosphoRSInput input)
        {
            SetOptions(input);
            SetModifications(input);
            SetSpectra(input);
        }

        public PhosphoRSOutput Run()
        {
            var dataConection = new DataConection(this);
            var phosphoRS3 = dataConection.CreateThreadManagement();
            phosphoRS3.StartPTMLocalisation();
            var result = phosphoRS3.PTMResult;
            return ArrangeResult(result);
        }


        private List<Tuple<PeptideSpectrumMatch, List<Tuple<int, List<int>>>>> _items;

        private List<AminoAcidModification> _aminoAcidModifications;

        private AminoAcidModification _phosphoModification;

        private char[] _phosphoTargetAminoAcids;

        private PhosphoRSOptions _options;

        private IList<Modification> _modifications;

        private IList<Spectrum> _spectra;

        private IDictionary<int, Spectrum> _spectrumMap;

        private IDictionary<int, Peptide> _peptideMap;

        private IDictionary<int, List<int>> _phosphoSiteMap;

        private void SetOptions(PhosphoRSInput input)
        {
            _options = input.Options;
        }

        private void SetModifications(PhosphoRSInput input)
        {
            _modifications = input.Modifications;

            _aminoAcidModifications = _modifications.Select(
                modification => new AminoAcidModification(
                    modification.Symbol,
                    modification.Name,
                    modification.Abbreviation,
                    modification.NeutralLossAbbreviation,
                    modification.MassDelta,
                    modification.NeutralLoss,
                    AminoAcidSequence.ParseAASequence(modification.TargetAminoAcids)
                )
            ).ToList();

            _phosphoModification = _aminoAcidModifications
                .Where(modification => modification.ID == _options.PhosphorylationSymbol)
                .First();

            _phosphoTargetAminoAcids = _phosphoModification.TargetAminoAcids
                .Select(a => a.OneLetterCode)
                .ToArray();
        }

        private void SetSpectra(PhosphoRSInput input)
        {
            _spectra = input.Spectra;

            _spectrumMap = new Dictionary<int, Spectrum>();
            _peptideMap = new Dictionary<int, Peptide>();
            _phosphoSiteMap = new Dictionary<int, List<int>>();

            _items = new List<Tuple<PeptideSpectrumMatch, List<Tuple<int, List<int>>>>>();

            var count = 0;
            foreach (var spectrum in input.Spectra)
            {            
                var rank = 0;    
                foreach (var peptide in spectrum.IdentifiedPhosphorPeptides)
                {
                    if (_options.MaxHitRank > 0 && rank >= _options.MaxHitRank)
                        break;
                    rank += 1;

                    var isoformMap = new List<Tuple<int, List<int>>>();

                    var aas = AminoAcidSequence.Create(
                        peptide.ID,
                        peptide.Sequence,
                        _aminoAcidModifications,
                        peptide.Modification
                    );

                    var psm = new PeptideSpectrumMatch(
                        peptide.ID,
                        spectrum.ActivationType,
                        spectrum.PrecursorCharge,
                        1,
                        spectrum.Peaks.ToArray(),
                        aas
                    );

                    var phosphoCount = peptide.Modification
                        .Skip(2).SkipLast(2)
                        .Where(c => c == _options.PhosphorylationSymbol)
                        .Count();
                    
                    if (phosphoCount == 0)
                        continue;

                    var targetSites = peptide.Sequence
                        .Select((c, i) => _phosphoTargetAminoAcids.Contains(c) ? i : -1)
                        .Where(i => i >= 0)
                        .ToList();

                    targetSites.Combinations(phosphoCount).Take(_options.MaxIsoformCount).ForEach(sites =>
                    {              
                        var list = sites.ToList();         
                        isoformMap.Add(
                            new Tuple<int, List<int>>(
                                count,
                                list
                            )
                        );

                        _spectrumMap.Add(count, spectrum);
                        _peptideMap.Add(count, peptide);
                        _phosphoSiteMap.Add(count, list);
                        count += 1;
                    });

                    _items.Add(Tuple.Create<PeptideSpectrumMatch, List<Tuple<int, List<int>>>>(
                        psm,
                        isoformMap
                    ));
                }
            }
        }

        private PhosphoRSOutput ArrangeResult(PTMResultClass result)
        {
            var output = new PhosphoRSOutput();
            output.Options = _options;
            output.Modifications = _modifications;

            var peptideMap = new Dictionary<int, PeptideResult>();

            _spectra.ForEach(s => 
            {
                var sr = new SpectrumResult(
                    id: s.ID,
                    name: s.Name,
                    precursorCharge: s.PrecursorCharge,
                    activationType: s.ActivationType
                );
                s.IdentifiedPhosphorPeptides.ForEach(p => 
                {
                    var pr = new PeptideResult(
                        id: p.ID,
                        sequence: p.Sequence
                    );
                    sr.Peptides.Add(pr);
                    peptideMap.Add(pr.ID, pr);
                });
                output.Spectra.Add(sr);
            });

            result.IsoformGroupList.ForEach(ig =>
            {
                if (ig.Error)
                {
                    var id = ig.PeptideIDs?.FirstOrDefault();
                    if (id != null)
                    {
                        var peptide = _peptideMap[id.Value];
                        Console.WriteLine($"Peptide {peptide.ID} {peptide.Sequence}({peptide.Modification}): {ig.Message}");
                    }   
                    else 
                    {
                        Console.WriteLine(ig.Message);
                    }
                    return;
                }
                
                var pr = peptideMap[_peptideMap[ig.Peptides.First().ID].ID];
                ig.Peptides.OrderByDescending(p => p.Score).ForEach(p =>
                {                    
                    pr.Isoforms.Add(
                        new IsoformPrediction(
                            id: p.ID,
                            sites: _phosphoSiteMap[p.ID].Select(i => i + 1).ToList(),
                            score: p.Score,
                            probability: p.Probability
                        )
                    );
                });
                ig.SiteProbabilities.ForEach(s =>
                {
                    pr.SitePrediction.Add(
                        new SitePrediction(
                            position: s.SequencePosition,                           
                            probability: s.Probability
                        )
                    );
                });
            });

            return output;
        }


        internal class DataConection : ThreadManagement.IDataConection
        {
            public DataConection(PhosphoRSDataSource source)
            {
                _source = source;
            }

            public ThreadManagement CreateThreadManagement()
            {
                var cts = new CancellationTokenSource();

                progressMessageQueue = new BlockingCollection<ThreadManagement.progressMessage>(new ConcurrentQueue<ThreadManagement.progressMessage>());
                var totalNumberOfSpectra = _source._spectra.Count;
                
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ThreadManagement.progressMessage msg;
                        double lastProgress;

                        lastProgress = -1;
                        while (!progressMessageQueue.IsCompleted)
                        {
                            msg = progressMessageQueue.Take();
                            if (msg.type == ThreadManagement.progressMessage.typeOfMessage.stringMessage)
                            {
                                if (msg.message != null)
                                {
                                    // Console.WriteLine(msg.message);
                                }
                            }
                            else
                            {
                                var currentProgress = msg.spectraProcessed / totalNumberOfSpectra;

                                if (currentProgress >= lastProgress + 0.001)
                                {
                                    lastProgress = currentProgress;
                                    // Console.Write(lastProgress * 100);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                });

                var phosphoRS3 = new ThreadManagement(
                    this,
                    m_searchCancel: cts,
                    maxIsoformCount: _source._options.MaxIsoformCount,
                    maxPTMCount: _source._options.MaxPTMCount,
                    scoreNLPeaksToo: _source._options.ScoreNeutralLoss.ToString(),
                    masstolerance: _source._options.MassTolerance,
                    ptmAminoAcidModification: _source._phosphoModification,
                    totalNumberOfSpectra: totalNumberOfSpectra
                );

                return phosphoRS3;
            }

            private readonly PhosphoRSDataSource _source;

            private int currentNumber = 0;

            private BlockingCollection<ThreadManagement.progressMessage> progressMessageQueue;

            List<ThreadManagement.SpectraPackageItem> ThreadManagement.IDataConection.GetNewDataPackage(
                int maxSizeOfPackage,
                out int numberOfSpectraPacked,
                out int numberOfPeptidesPacked
            )
            {
                numberOfSpectraPacked = 0;
                numberOfPeptidesPacked = 0;

                if (currentNumber >= _source._items.Count)
                    return null;

                var package = new List<ThreadManagement.SpectraPackageItem>();
                for (int i = currentNumber; i < _source._items.Count && i - currentNumber < maxSizeOfPackage; i++)
                {
                    package.Add(new ThreadManagement.SpectraPackageItem(_source._items[i].Item1, 1 / _source._items.Count, _source._items[i].Item2));
                }

                currentNumber += package.Count;
                numberOfSpectraPacked = package.Count;
                numberOfPeptidesPacked = package.Count;
                return package;
            }

            BlockingCollection<ThreadManagement.progressMessage> ThreadManagement.IDataConection.GetProgressMessageQueue()
            {
                return progressMessageQueue;
            }
        }
    }
}