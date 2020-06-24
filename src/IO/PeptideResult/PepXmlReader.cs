using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace PhosphoRS.IO.PeptideResult
{
    public class PepXmlReader : IPeptideResultReader
    {
        private readonly XmlReader _reader;

        public PepXmlReader(Stream stream)
            : this(XmlReader.Create(stream))
        {
        }

        public PepXmlReader(TextReader reader)
            : this(XmlReader.Create(reader))
        {
        }

        public PepXmlReader(string path)
            : this(XmlReader.Create(path))
        {
        }

        public PepXmlReader(XmlReader reader)
        {
            _reader = reader;
        }

        public virtual void Close() => _reader.Close();

        public void Dispose() => _reader.Dispose();

        public SpectrumQuery Read()
        {
            SpectrumQuery spectrumQuery = null;

            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    if (_reader.Name == "spectrum_query")
                    {
                        if (spectrumQuery == null)
                        {
                            spectrumQuery = new SpectrumQuery()
                            {
                                Spectrum = _reader["spectrum"],
                                Charge = int.Parse(_reader["assumed_charge"])
                            };
                            
                            if (_reader.IsEmptyElement)
                                break;                 
                        }                        
                    }

                    if (_reader.Name == "search_hit")
                    {
                        spectrumQuery?.SearchResults?.Add(new SearchHit()
                        {
                            Rank = int.Parse(_reader["hit_rank"]),
                            Peptide = _reader["peptide"]
                        });                        
                    }

                    if (_reader.Name == "mod_aminoacid_mass")
                    {
                        spectrumQuery?.SearchResults?.LastOrDefault()
                            ?.Modifications?.Add(new Modification()
                            {
                                Position = int.Parse(_reader["position"]),
                                Mass = double.Parse(_reader["mass"])
                            });
                    }

                    if (_reader.Name == "search_score")
                    {
                        spectrumQuery?.SearchResults?.LastOrDefault()
                            ?.SearchScores?.Add(
                                key: _reader["name"],
                                value: double.Parse(_reader["value"])
                            );
                    }
                }

                if (_reader.NodeType == XmlNodeType.EndElement)
                {
                    if (_reader.Name == "spectrum_query")
                        break;
                }
            }

            return spectrumQuery;
        }
    }
}