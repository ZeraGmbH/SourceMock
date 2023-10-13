using SourceMock.Model;

namespace SourceMock.Actions.Source
{
    public static partial class QuantizedRangeExtension
    {
        public static bool IsIncluded(this QuantizedRange range, double value)
        {
            var mod = value % range.QuantisationDistance;

            return
                !(value < range.LowerEndpoint || value > range.UpperEndpoint);
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