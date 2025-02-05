using MongoDB.Bson.Serialization.Attributes;
using SerialPortProxy;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Configuration of a custom serial port - protocol
/// defined by third-party.
/// </summary>
public class CustomSerialPortConfiguration
{
    /// <summary>
    /// Configuration of a serial port connection.
    /// </summary>
    [BsonElement("serialPort")]
    public SerialPortConfiguration? SerialPort { get; set; }
}