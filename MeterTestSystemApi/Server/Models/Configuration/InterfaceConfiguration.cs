using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Connectivity configuration.
/// </summary>
public class InterfaceConfiguration
{
    /// <summary>
    /// Configuration of a serial port connection.
    /// </summary>
    [NotNull, Required]
    [BsonElement("serial")]
    public SerialPortConfiguration SerialPort { get; set; } = new();
}
