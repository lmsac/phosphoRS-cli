using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using IMP.PhosphoRS;
using PhosphoRS.Wrapper;

namespace PhosphoRS.IO
{
    public class PhosphoRSInputXmlReader : IPhosphoRSInputReader
    {
        private readonly XmlReader _reader;

        public PhosphoRSInputXmlReader(Stream stream)
            : this(XmlReader.Create(stream))
        {
        }

        public PhosphoRSInputXmlReader(TextReader reader)
            : this(XmlReader.Create(reader))
        {
        }

        public PhosphoRSInputXmlReader(string path)
            : this(XmlReader.Create(path))
        {
        }

        public PhosphoRSInputXmlReader(XmlReader reader)
        {
            _reader = reader;
        }

        public virtual void Close() => _reader.Close();

        public void Dispose() => _reader.Dispose();

        public PhosphoRSInput Read()
        {
            var input = new PhosphoRSInput();

            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    if (_reader.Name == "Spectra")
                    {
                        if (input.Spectra == null)
                            input.Spectra = new List<Spectrum>();
                    }

                    if (_reader.Name == "Spectrum")
                    {
                        input.Spectra?.Add(new Spectrum(
                            id: int.Parse(_reader["ID"]),
                            name: _reader["Name"],
                            precursorCharge: int.Parse(_reader["PrecursorCharge"]),
                            activationType: SpectrumTypeHelper.ParseSpectrumType(_reader["ActivationTypes"]),
                            peaks: null
                        ));
                    }

                    if (_reader.Name == "Peaks")
                    {
                        var spectrum = input.Spectra?.LastOrDefault();
                        if (spectrum != null)
                            spectrum.Peaks = Peak.ParsePeaks(_reader.ReadElementContentAsString());
                    }

                    if (_reader.Name == "IdentifiedPhosphorPeptides")
                    {
                        var spectrum = input.Spectra?.LastOrDefault();
                        if (spectrum != null && spectrum.IdentifiedPhosphorPeptides == null)
                            spectrum.IdentifiedPhosphorPeptides = new List<Peptide>();
                    }

                    if (_reader.Name == "Peptide")
                    {
                        input.Spectra?.LastOrDefault()?.IdentifiedPhosphorPeptides?.Add(new Peptide(
                            id: int.Parse(_reader["ID"]),
                            sequence: _reader["Sequence"],
                            modification: _reader["ModificationInfo"]
                        ));
                    }

                    if (_reader.Name == "MassTolerance")
                    {
                        if (input.Options == null)
                            input.Options = new PhosphoRSOptions();
                        input.Options.MassTolerance = double.Parse(_reader["Value"]);
                    }

                    if (_reader.Name == "MaxIsoformCount")
                    {
                        if (input.Options == null)
                            input.Options = new PhosphoRSOptions();
                        input.Options.MaxIsoformCount = int.Parse(_reader["Value"]);
                    }

                    if (_reader.Name == "MaxPTMCount")
                    {
                        if (input.Options == null)
                            input.Options = new PhosphoRSOptions();
                        input.Options.MaxPTMCount = int.Parse(_reader["Value"]);
                    }

                    if (_reader.Name == "ScoreNeutralLoss")
                    {
                        if (input.Options == null)
                            input.Options = new PhosphoRSOptions();
                        input.Options.ScoreNeutralLoss = bool.Parse(_reader["Value"]);
                    }

                    if (_reader.Name == "Phosphorylation")
                    {
                        if (input.Options == null)
                            input.Options = new PhosphoRSOptions();
                        input.Options.PhosphorylationSymbol = char.Parse(_reader["Symbol"]);
                    }

                    if (_reader.Name == "ModificationInfos")
                    {
                        if (input.Modifications == null)
                            input.Modifications = new List<Modification>();
                    }

                    if (_reader.Name == "ModificationInfo")
                    {
                        var s = _reader["Value"]?.Split(':');
                        input.Modifications?.Add(new Modification(
                            symbol: char.Parse(_reader["Symbol"]),
                            name: s[1],
                            abbreviation: s[2],
                            massDelta: double.Parse(s[3]),
                            neutralLossAbbreviation: s[4],
                            neutralLoss: double.Parse(s[5]),
                            targetAminoAcids: s[6]
                        ));
                    }
                }
            }
            return input;
        }
    }
}
