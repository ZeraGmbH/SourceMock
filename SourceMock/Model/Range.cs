namespace SourceMock.Model
{
    public class Range
    {
        public double LowerEndpoint { get; set; }
        public double UpperEndpoint { get; set; }

        public Range(double lowerEndpoint, double upperEndpoint)
        {
            LowerEndpoint = lowerEndpoint;
            UpperEndpoint = upperEndpoint;
        }

        public Range() { }
    }
}