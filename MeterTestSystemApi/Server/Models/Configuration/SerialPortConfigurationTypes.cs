using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Supported types of a serial port connection.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SerialPortConfigurationTypes
{
    /// <summary>
    /// Physical serial port, e.g. COM11 or /dev/ttyUSB0.
    /// </summary>
    Device = 0,

    /// <summary>
    /// Network connection, i.e. Adress:Port.
    /// </summary>
    Network = 1,

    /// <summary>
    /// Use internal mock.
    /// </summary>
    Mock = 2
}