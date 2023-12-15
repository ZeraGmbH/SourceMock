using System.Text.Json.Serialization;

namespace SerialPortProxy;

/// <summary>
/// Possible error conditions when using a serial port connecttion
/// to a meter device.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SerialPortErrorCodes
{
    /// <summary>
    /// The operation has been cancelled by the user.
    /// </summary>
    SerialPortAborted,

    /// <summary>
    /// An invalid command has been used.
    /// </summary>
    SerialPortBadRequest,

    /// <summary>
    /// Command not correctly confirem in a appropriate time frame.
    /// </summary>
    SerialPortTimeOut,
}
