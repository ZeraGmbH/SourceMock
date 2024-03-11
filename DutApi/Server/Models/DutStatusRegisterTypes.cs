using System.Text.Json.Serialization;

namespace DutApi.Models;

/// <summary>
/// Register types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DutStatusRegisterTypes
{
    /// <summary>
    /// Current of first phase.
    /// </summary>
    Current1 = 4,

    /// <summary>
    /// Current of second phase.
    /// </summary>
    Current2 = 5,

    /// <summary>
    /// Current of third phase.
    /// </summary>
    Current3 = 6,

    /// <summary>
    /// DC current.
    /// </summary>
    CurrentDC = 7,

    /// <summary>
    /// Power of first phase.
    /// </summary>
    Power1 = 8,

    /// <summary>
    /// Power of second phase.
    /// </summary>
    Power2 = 9,

    /// <summary>
    /// Power of thirdphase.
    /// </summary>
    Power3 = 10,

    /// <summary>
    /// DC power.
    /// </summary>
    PowerDC = 11,

    /// <summary>
    /// Serial number.
    /// </summary>
    Serial = 12,

    /// <summary>
    /// Voltage of first phase.
    /// </summary>
    Voltage1 = 0,

    /// <summary>
    /// Voltage of second phase.
    /// </summary>
    Voltage2 = 1,

    /// <summary>
    /// Voltage of third phase.
    /// </summary>
    Voltage3 = 2,

    /// <summary>
    /// DC voltage.
    /// </summary>
    VoltageDC = 3,
}