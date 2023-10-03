namespace SourceMock.Actions.Source
{
    public static partial class RangeExtension
    {
        public static bool IsIncluded(this Model.Range range, double value)
        {
            return !(value < range.LowerEndpoint || value > range.UpperEndpoint);
        }

        public static bool IsIncluded(this List<Model.Range> ranges, double value)
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