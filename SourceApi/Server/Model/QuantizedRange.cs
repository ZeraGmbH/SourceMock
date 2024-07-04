using SharedLibrary.DomainSpecific;

namespace SourceApi.Model
{
    public class QuantizedRange<T> where T : struct, IDomainSpecificNumber
    {
        public T Min { get; set; }
        public T Max { get; set; }
        public T PrecisionStepSize { get; set; }


        public QuantizedRange(T min, T max, T precisionStepSize)
        {
            Min = min;
            Max = max;
            PrecisionStepSize = precisionStepSize;
        }

        public QuantizedRange() { }
    }

}