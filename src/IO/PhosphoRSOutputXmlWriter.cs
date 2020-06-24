using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using IMP.PhosphoRS;
using PhosphoRS.Wrapper;
using PhosphoRS.Util;

namespace PhosphoRS.IO
{
    public class PhosphoRSOutputXmlWriter : IPhosphoRSOutputWriter
    {
        private readonly XmlWriter _writer;

        public PhosphoRSOutputXmlWriter(Stream stream)
            : this(XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true }))
        {
        }

        public PhosphoRSOutputXmlWriter(TextWriter writer)
            : this(XmlWriter.Create(writer, new XmlWriterSettings() { Indent = true }))
        {
        }

        public PhosphoRSOutputXmlWriter(string path)
            : this(XmlWriter.Create(path, new XmlWriterSettings() { Indent = true }))
        {
        }

        public PhosphoRSOutputXmlWriter(XmlWriter writer)
        {
            _writer = writer;
        }

        public virtual void Close() => _writer.Close();

        public void Dispose() => _writer.Dispose();

        public void Write(PhosphoRSOutput output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            _writer.WriteStartDocument();
            _writer.WriteStartElement("PhosphoRS_Results");

            WriteGlobalParameters(output);

            WriteSpectra(output);

            _writer.WriteEndElement();
            _writer.WriteEndDocument();
        }

        private void WriteGlobalParameters(PhosphoRSOutput output)
        {
            _writer.WriteStartElement("GlobalParameters");

            _writer.WriteStartElement("MassTolerance");
            _writer.WriteAttributeString("Value", output.Options.MassTolerance.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("Modifications");
            output.Modifications.ForEach(modification =>
            {
                _writer.WriteStartElement("Modification");
                _writer.WriteAttributeString("ID", modification.Symbol.ToString());
                _writer.WriteAttributeString("Name", modification.Name.ToString());
                _writer.WriteAttributeString("MassDelta", modification.MassDelta.ToString());
                _writer.WriteEndElement();
            });
            _writer.WriteEndElement();

            _writer.WriteStartElement("Phosphorylation");
            _writer.WriteAttributeString("ID", output.Options.PhosphorylationSymbol.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("MaxIsoformCount");
            _writer.WriteAttributeString("Value", output.Options.MaxIsoformCount.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("MaxPTMCount");
            _writer.WriteAttributeString("Value", output.Options.MaxPTMCount.ToString());
            _writer.WriteEndElement();

            _writer.WriteStartElement("ScoreNeutralLoss");
            _writer.WriteAttributeString("Value", output.Options.ScoreNeutralLoss.ToString());
            _writer.WriteEndElement();

            _writer.WriteEndElement();
        }

        private void WriteSpectra(PhosphoRSOutput output)
        {
            _writer.WriteStartElement("Spectra");

            output.Spectra.ForEach(spectrum =>
            {
                _writer.WriteStartElement("Spectrum");
                _writer.WriteAttributeString("ID", spectrum.ID.ToString());
                _writer.WriteAttributeString("Name", spectrum.Name?.ToString());
                _writer.WriteAttributeString("PrecursorCharge", spectrum.PrecursorCharge.ToString());
                _writer.WriteAttributeString("ActivationType", spectrum.ActivationType.ToString());

                _writer.WriteStartElement("Peptides");
                spectrum.Peptides.ForEach(peptide =>
                {
                    _writer.WriteStartElement("Peptide");
                    _writer.WriteAttributeString("ID", peptide.ID.ToString());

                    _writer.WriteStartElement("SitePrediction");
                    peptide.SitePrediction.ForEach(site =>
                    {
                        _writer.WriteStartElement("Site");
                        _writer.WriteAttributeString("SeqPos", site.Position.ToString());
                        _writer.WriteAttributeString("SiteProb", site.Probability.ToString());
                        _writer.WriteEndElement();
                    });
                    _writer.WriteEndElement();

                    _writer.WriteStartElement("Isoforms");
                    peptide.Isoforms.ForEach(isoform =>
                    {
                        _writer.WriteStartElement("Isoform");
                        _writer.WriteAttributeString("ID", isoform.ID.ToString());
                        _writer.WriteAttributeString("PepScore", isoform.Score.ToString());
                        _writer.WriteAttributeString("PepProb", isoform.Probability.ToString());

                        _writer.WriteStartElement("PhosphoSites");
                        isoform.Sites.ForEach(site =>
                        {
                            _writer.WriteStartElement("PhosphoSite");
                            _writer.WriteAttributeString("SeqPos", site.ToString());
                            _writer.WriteEndElement();
                        });
                        _writer.WriteEndElement();

                        _writer.WriteEndElement();
                    });
                    _writer.WriteEndElement();

                    _writer.WriteEndElement();
                });
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            });

            _writer.WriteEndElement();
        }
    }
}
