using MongoDB.Bson.Serialization.Attributes;
using SourceApi.Model.Configuration;

namespace ZIFApi.Models;

/// <summary>
/// ZIF socket configuration.
/// </summary>
public class ZIFConfiguration
{
    /// <summary>
    /// Protocol used to communicate with a ZIF device.
    /// </summary>
    [BsonElement("type")]
    public SupportedZIFProtocols Type { get; set; }

    /// <summary>
    /// Configuration of a serial port connection.
    /// </summary>
    [BsonElement("serialPort")]
    public SerialPortConfiguration? SerialPort { get; set; }
}