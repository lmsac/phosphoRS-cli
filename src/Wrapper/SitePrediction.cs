namespace PhosphoRS.Wrapper
{
    public class SitePrediction
    {
        public SitePrediction(int position, double probability)
        {
            Position = position;
            Probability = probability;
        }

        public int Position { get; set; }

        public double Probability { get; set; }
    }
}