using MongoDB.Bson.Serialization.Attributes;
using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Connectivity configuration.
/// </summary>
public class InterfaceConfiguration
{
    /// <summary>
    /// Configuration of a serial port connection.
    /// </summary>
    [BsonElement("serial")]
    public SerialPortConfiguration? SerialPort { get; set; }
}
