using System.Collections.Generic;
using IMP.PhosphoRS;

namespace PhosphoRS.Wrapper
{
    public class Modification
    {
        public Modification(
            char symbol, string name, string abbreviation, double massDelta,
            string neutralLossAbbreviation, double neutralLoss,
            string targetAminoAcids
        )
        {
            Symbol = symbol;
            Name = name;
            Abbreviation = abbreviation;
            MassDelta = massDelta;
            NeutralLossAbbreviation = neutralLossAbbreviation;
            NeutralLoss = neutralLoss;
            TargetAminoAcids = targetAminoAcids;
        }

        public char Symbol { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public double MassDelta { get; set; }

        public string NeutralLossAbbreviation { get; set; }

        public double NeutralLoss { get; set; }

        public string TargetAminoAcids { get; set; }

        public static Modification PhosphoSTY => new Modification(
            symbol: 'p',
            name: "Phospho",
            abbreviation: "Phospho",
            massDelta: 79.966331,
            neutralLossAbbreviation: "PhosphoLoss",
            neutralLoss: 97.976896,
            targetAminoAcids: "STY"
        );

        public static Modification OxidationM => new Modification(
            symbol: 'm',
            name: "Oxidation",
            abbreviation: "Oxidation",
            massDelta: 15.994919,
            neutralLossAbbreviation: "null",
            neutralLoss: 0,
            targetAminoAcids: "M"
        );

        public static Modification CarbamidomethylC => new Modification(
            symbol: 'c',
            name: "Carbamidomethyl",
            abbreviation: "Carbamidomethyl",
            massDelta: 57.0215,
            neutralLossAbbreviation: "null",
            neutralLoss: 0,
            targetAminoAcids: "C"
        );
    }
}