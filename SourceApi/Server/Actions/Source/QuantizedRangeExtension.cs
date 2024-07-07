using SharedLibrary.DomainSpecific;
using SourceApi.Model;

namespace SourceApi.Actions.Source;

public static partial class QuantizedRangeExtension
{
    public static bool IsIncluded<T>(this QuantizedRange<T> range, T value) where T : struct, IDomainSpecificNumber<T>
    {
        var mod = value % range.PrecisionStepSize;
        var mod2 = (mod - range.PrecisionStepSize).Abs();
        var epsilon = T.Create(1E-12);

        return !(value < range.Min || value > range.Max) && (mod <= epsilon || mod2 <= epsilon);
    }

    public static bool IsIncluded<T>(this List<QuantizedRange<T>> ranges, T value) where T : struct, IDomainSpecificNumber<T>
        => ranges.FindIndex(r => r.IsIncluded(value)) >= 0;
}