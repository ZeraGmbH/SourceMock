using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Services;

/// <summary>
/// All protocols available for a serial port connection.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SerialProbeProtocols
{
    /// <summary>
    /// FG30x frequency generator.
    /// </summary>
    FG30x = 0,

    /// <summary>
    /// MT768 compatible.
    /// </summary>
    MT768 = 1,

    /// <summary>
    /// Power Master Model 8121, ZIF socket.
    /// </summary>
    PM8181 = 2,

    /// <summary>
    /// ESVB/ESCB burden.
    /// </summary>
    ESxB = 3,
}