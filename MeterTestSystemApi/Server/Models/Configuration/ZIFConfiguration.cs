using MongoDB.Bson.Serialization.Attributes;
using SourceApi.Model.Configuration;
using ZIFApi.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// ZIF socket configuration.
/// </summary>
public class ZIFConfiguration
{
    /// <summary>
    /// Protocol used to communicate with a ZIF device.
    /// </summary>
    [BsonElement("type")]
    public SupportedZIFDevices Type { get; set; }

    /// <summary>
    /// Configuration of a serial port connection.
    /// </summary>
    [BsonElement("serialPort")]
    public SerialPortConfiguration? SerialPort { get; set; }
}
