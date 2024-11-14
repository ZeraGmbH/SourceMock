using System.Text.RegularExpressions;

namespace BurdenApi.Actions;

/// <summary>
/// Static helper functions.
/// </summary>
public class BurdenUtils
{
    private static readonly Regex _RangePattern = new("^([^/]+)(/(3|v3))?$");

    /// <summary>
    /// Analyze the range string and provide the corresponding value.
    /// </summary>
    /// <param name="range">Range desciption.</param>
    /// <returns>Corresponding value.</returns>
    public static double ParseRange(string range)
    {
        // Analyse the range pattern - assume some number optional followed by a scaling.
        var match = _RangePattern.Match(range);

        if (!match.Success) throw new ArgumentException(range, nameof(range));

        var rawRange = double.Parse(match.Groups[1].Value);

        // Apply the scaling.
        return match.Groups[3].Value switch
        {
            "3" => rawRange / 3,
            "v3" => rawRange / Math.Sqrt(3),
            _ => rawRange,
        };
    }
}
