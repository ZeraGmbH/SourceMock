using System.Text.Json.Serialization;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CalibrationAlgorithms
{
    /// <summary>
    /// Use system wide default.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Adjust one calibration at a time.
    /// </summary>
    SingleStep = 1,

    /// <summary>
    /// Do a interval reduction on all four calibration values.
    /// </summary>
    Interval = 2
}