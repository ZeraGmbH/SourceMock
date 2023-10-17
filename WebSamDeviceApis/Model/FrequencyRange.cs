namespace SourceMock.Model
{
    public class FrequencyRange : QuantizedRange
    {
        public FrequencyMode Mode { get; set; }

        public FrequencyRange(double lowerEndpoint, double upperEndpoint, double quantisationDistance, FrequencyMode mode) :
            base(lowerEndpoint, upperEndpoint, quantisationDistance)
        {
            Mode = mode;
        }

        public FrequencyRange()
        {

        }
    }
}