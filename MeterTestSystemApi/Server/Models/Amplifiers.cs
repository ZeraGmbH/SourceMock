using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models;

/// <summary>
/// All types of amplifiers.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Amplifiers
{
    /// <summary>
    /// First auxiliary.
    /// </summary>
    Auxiliary1 = 0,

    /// <summary>
    /// Second auxiliary.
    /// </summary>
    Auxiliary2 = 1,

    /// <summary>
    /// First current.
    /// </summary>
    Current1 = 2,

    /// <summary>
    /// Second current.
    /// </summary>
    Current2 = 3,

    /// <summary>
    /// Third current.
    /// </summary>
    Current3 = 4,

    /// <summary>
    /// First voltage.
    /// </summary>
    Voltage1 = 5,

    /// <summary>
    /// Second voltage.
    /// </summary>
    Voltage2 = 6,

    /// <summary>
    /// Third voltage.
    /// </summary>
    Voltage3 = 7,
}
