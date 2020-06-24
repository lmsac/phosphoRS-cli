using System;
using System.CommandLine;
using System.CommandLine.Invocation;

using PhosphoRS.Wrapper;
using PhosphoRS.IO;

namespace PhosphoRS.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Option inputOption = new Option(
                "--input", 
                "Input file (.XML). This option will be used first.", 
                new Argument<string>()
            );
            inputOption.AddAlias("-i");

            Option outputOption = new Option(
                "--output", 
                "Output path (.XML, .JSON or .CSV).", 
                new Argument<string>()
            );
            outputOption.AddAlias("-o");

            Option peptideOption = new Option(
                "--peptide", 
                "Peptide identification result file (.PepXML). This option will be ignored if --input is given.", 
                new Argument<string>()
            );
            peptideOption.AddAlias("-p");
            Option spectraOption = new Option(
                "--spectra", 
                "MS2 Spectra file (.MGF). This option will be ignored if --input is given.", 
                new Argument<string>()
            );
            spectraOption.AddAlias("-s");

            Option activationOption = new Option(
                "--activation", 
                "Activation type (CID, HCD, ECD or ETD). Default CID. This option will be ignored if --input is given.", 
                new Argument<string>("CID")
            );
            activationOption.AddAlias("-a");            
            Option toleranceOption = new Option(
                "--tolerance", 
                "Mass tolerance. Default 0.02. This option will be ignored if --input is given.", 
                new Argument<double>(defaultValue: 0.02)
            );
            toleranceOption.AddAlias("-t");
            Option maxIsoformOption = new Option(
                "--max-isoform", 
                "Maximun number of isoforms. Default 200. This option will be ignored if --input is given.",
                 new Argument<int>(defaultValue: 200)
            );
            maxIsoformOption.AddAlias("-I");
            Option maxPTMOption = new Option(
                "--max-ptm", 
                "Maximun number of PTMs. Default 20. This option will be ignored if --input is given.", 
                new Argument<int>(defaultValue: 20)
            );
            maxPTMOption.AddAlias("-P");
            Option neutralLossOption = new Option(
                "--neutral-loss", 
                "Score neutral loss. Default on. This option will be ignored if --input is given.", 
                new Argument<bool>(defaultValue: true)
            );
            neutralLossOption.AddAlias("-n");

            var rootCommand = new RootCommand(                
                description: "PhosphoRS Commandline Tool"
            );
            rootCommand.AddOption(inputOption);
            rootCommand.AddOption(outputOption);
            rootCommand.AddOption(peptideOption);
            rootCommand.AddOption(spectraOption);
            rootCommand.AddOption(activationOption);
            rootCommand.AddOption(toleranceOption);
            rootCommand.AddOption(maxIsoformOption);
            rootCommand.AddOption(maxPTMOption);
            rootCommand.AddOption(neutralLossOption);

            rootCommand.Handler = CommandHandler
                .Create(typeof(Program).GetMethod(nameof(Run)), () => new Program());

            rootCommand.InvokeAsync(args).Wait();
        }

        public void Run(
            string input, string peptide, string spectra, string output,
            string activation, double tolerance, int maxIsoform, int maxPTM, bool neutralLoss
        )
        {
            IPhosphoRSInputReader reader = null;            
            if (!string.IsNullOrWhiteSpace(input))
            {
                if (string.IsNullOrWhiteSpace(output))
                    output = System.IO.Path.ChangeExtension(input, ".out.xml");                
                reader = GetInputReader(input);
            }            
            if (reader == null && !string.IsNullOrWhiteSpace(peptide))
            {
                if (string.IsNullOrWhiteSpace(output))
                    output = System.IO.Path.ChangeExtension(peptide, ".out.xml");
                if (!string.IsNullOrWhiteSpace(spectra))
                    reader = GetInputReader(
                        peptide, spectra, 
                        CreateOptions(activation, tolerance, maxIsoform, maxPTM, neutralLoss)
                    );
            }            
            if (reader == null)
            {                
                Console.WriteLine("Invalid options. Use -h to see help.");
                return;
            }

            PhosphoRSInput data;           
            using (reader)
                data = reader.Read(); 

            var source = new PhosphoRSDataSource(data);
            var result = source.Run();
           
            using (var writer = GetOutputWriter(output))
                writer.Write(result);
        }

        private PhosphoRSOptions CreateOptions(
            string activation, double tolerance, int maxIsoform, int maxPTM, bool neutralLoss
        )
        {
            return new PhosphoRSOptions()
            {
                ActivationType = SpectrumTypeHelper.ParseSpectrumType(activation),
                MassTolerance = tolerance,
                MaxIsoformCount = maxIsoform,
                MaxPTMCount = maxPTM,
                ScoreNeutralLoss = neutralLoss
            };
        }

        private IPhosphoRSInputReader GetInputReader(string peptide, string spectra, PhosphoRSOptions options = null)
        {
            IO.PeptideResult.IPeptideResultReader peptideReader;
            {
                var ext = System.IO.Path.GetExtension(peptide);
                if (ext.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                    peptideReader = new IO.PeptideResult.PepXmlReader(peptide);
                else if (ext.Equals(".pepXML", StringComparison.InvariantCultureIgnoreCase))
                    peptideReader = new IO.PeptideResult.PepXmlReader(peptide);
                else
                    peptideReader = new IO.PeptideResult.PepXmlReader(peptide);
            }
            IO.Spectral.ISpectraReader spectraReader;
            {
                var ext = System.IO.Path.GetExtension(spectra);
                if (ext.Equals(".mgf", StringComparison.InvariantCultureIgnoreCase))
                    spectraReader = new IO.Spectral.MgfReader(spectra);
                else if (ext.Equals(".mzXML", StringComparison.InvariantCultureIgnoreCase))
                    throw new NotImplementedException(".mzXML");                    
                else
                    spectraReader = new IO.Spectral.MgfReader(spectra);
            }
            return new IO.PeptideResultSpectraInputReader(peptideReader, spectraReader, options);
        }

        private IPhosphoRSInputReader GetInputReader(string input)
        {
            var ext = System.IO.Path.GetExtension(input);
            if (ext.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                return new PhosphoRSInputXmlReader(input);
            else
                return new PhosphoRSInputXmlReader(input);
        }

        private IPhosphoRSOutputWriter GetOutputWriter(string output)
        {
            var ext = System.IO.Path.GetExtension(output);
            if (ext.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                return new PhosphoRSOutputXmlWriter(output);
            else if (ext.Equals(".json", StringComparison.InvariantCultureIgnoreCase))
                return new PhosphoRSOutputJsonWriter(output);
            else if (ext.Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
                return new PhosphoRSOutputCsvWriter(output);
            else
                return new PhosphoRSOutputXmlWriter(output);
        }
    }
}
