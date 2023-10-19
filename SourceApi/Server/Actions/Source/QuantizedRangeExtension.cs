using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.Source
{
    public static partial class QuantizedRangeExtension
    {
        public static bool IsIncluded(this QuantizedRange range, double value)
        {
            var mod = value % range.PrecisionStepSize;
            var mod2 = Math.Abs(mod - range.PrecisionStepSize);

            const double epsilon = 1E-12;

            return
                !(value < range.Min || value > range.Max) &&
                (mod <= epsilon || mod2 <= epsilon);
        }

        public static bool IsIncluded(this List<QuantizedRange> ranges, double value)
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