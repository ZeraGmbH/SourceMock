using SharedLibrary.DomainSpecific;
using SourceApi.Model;

namespace SourceApi.Actions.Source
{
    public static partial class QuantizedRangeExtension
    {
        public static bool IsIncluded(this QuantizedRange<Current> range, Current value)
        {
            var mod = value % range.PrecisionStepSize;
            var mod2 = (mod - range.PrecisionStepSize).Abs();

            Current epsilon = new(1E-12);

            return
                !(value < range.Min || value > range.Max) &&
                (mod <= epsilon || mod2 <= epsilon);
        }

        public static bool IsIncluded(this QuantizedRange<Voltage> range, Voltage value)
        {
            var mod = value % range.PrecisionStepSize;
            var mod2 = (mod - range.PrecisionStepSize).Abs();

            Voltage epsilon = new(1E-12);

            return
                !(value < range.Min || value > range.Max) &&
                (mod <= epsilon || mod2 <= epsilon);
        }

        public static bool IsIncluded(this List<QuantizedRange<Voltage>> ranges, Voltage value)
        {
            foreach (var range in ranges)
            {
                if (range.IsIncluded(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}