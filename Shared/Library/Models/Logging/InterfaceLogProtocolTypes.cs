using System.Text.Json.Serialization;

namespace SharedLibrary.Models.Logging;

/// <summary>
/// Physical type of the communication.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]

public enum InterfaceLogProtocolTypes
{
    /// <summary>
    /// Serial port.
    /// </summary>
    Com = 0,

    /// <summary>
    /// Network with TCP stream connection.
    /// </summary>
    Tcp = 1,

    /// <summary>
    /// Network with UDP packet connection.
    /// </summary>
    Udp = 2,

    /// <summary>
    /// Internally mocked interface.
    /// </summary>
    Mock = 3,
}