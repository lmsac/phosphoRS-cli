using IMP.PhosphoRS;

namespace PhosphoRS.Wrapper
{
    public class PhosphoRSOptions
    {
        public double MassTolerance { get; set; } = 0.02;

        public char PhosphorylationSymbol { get; set; } = '1';

        public int MaxIsoformCount { get; set; } = 200;

        public int MaxPTMCount { get; set; } = 20;

        public bool ScoreNeutralLoss { get; set; } = true;

        public SpectrumType ActivationType { get; set; } = SpectrumType.CID_CAD;

        public int MaxHitRank { get; set; } = 1;
    }
}