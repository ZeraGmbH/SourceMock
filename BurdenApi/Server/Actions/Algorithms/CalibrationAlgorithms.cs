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
    /// Starts with adjusting the fine calibration first.
    /// </summary>
    FineFirst = 2,
}