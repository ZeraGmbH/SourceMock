namespace SourceMock.Model
{
    public class QuantizedRange
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double PrecisionStepSize { get; set; }


        public QuantizedRange(double min, double max, double precisionStepSize)
        {
            Min = min;
            Max = max;
            PrecisionStepSize = precisionStepSize;
        }

        public QuantizedRange() { }
    }
}