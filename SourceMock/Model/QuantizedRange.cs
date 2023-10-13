namespace SourceMock.Model
{
    public class QuantizedRange
    {
        public double LowerEndpoint { get; set; }
        public double UpperEndpoint { get; set; }
        public double QuantisationDistance { get; set; }


        public QuantizedRange(double lowerEndpoint, double upperEndpoint, double quantisationDistance)
        {
            LowerEndpoint = lowerEndpoint;
            UpperEndpoint = upperEndpoint;
            QuantisationDistance = quantisationDistance;
        }

        public QuantizedRange() { }
    }
}