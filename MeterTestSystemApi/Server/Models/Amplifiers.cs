using System.Text.Json.Serialization;

namespace MeterTestSystemApi;

/// <summary>
/// All types of amplifiers.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Amplifiers
{
    /// <summary>
    /// First auxiliary.
    /// </summary>
    Auxiliary1,

    /// <summary>
    /// Second auxiliary.
    /// </summary>
    Auxiliary2,

    /// <summary>
    /// First current.
    /// </summary>
    Current1,

    /// <summary>
    /// Second current.
    /// </summary>
    Current2,

    /// <summary>
    /// Third current.
    /// </summary>
    Current3,

    /// <summary>
    /// First voltage.
    /// </summary>
    Voltage1,

    /// <summary>
    /// Second voltage.
    /// </summary>
    Voltage2,

    /// <summary>
    /// Third voltage.
    /// </summary>
    Voltage3,
}
