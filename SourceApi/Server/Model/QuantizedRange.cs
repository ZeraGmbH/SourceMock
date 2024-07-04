using SharedLibrary.DomainSpecific;

namespace SourceApi.Model
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
    public class QuantizedCurrentRange
    {
        public Current Min { get; set; }
        public Current Max { get; set; }
        public Current PrecisionStepSize { get; set; }


        public QuantizedCurrentRange(Current min, Current max, Current precisionStepSize)
        {
            Min = min;
            Max = max;
            PrecisionStepSize = precisionStepSize;
        }

        public QuantizedCurrentRange() { }
    }
    public class QuantizedVoltageRange
    {
        public Voltage Min { get; set; }
        public Voltage Max { get; set; }
        public Voltage PrecisionStepSize { get; set; }


        public QuantizedVoltageRange(Voltage min, Voltage max, Voltage precisionStepSize)
        {
            Min = min;
            Max = max;
            PrecisionStepSize = precisionStepSize;
        }

        public QuantizedVoltageRange() { }
    }
}