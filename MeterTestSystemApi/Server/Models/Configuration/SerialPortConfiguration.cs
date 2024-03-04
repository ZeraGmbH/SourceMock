using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Configuration of a serial port connection.
/// </summary>
public class SerialPortConfiguration
{
    /// <summary>
    /// Type of the impmentation to use.
    /// </summary>
    [NotNull, Required]
    [BsonElement("type")]
    public SerialPortConfigurationTypes ConfigurationType { get; set; }

    /// <summary>
    /// Endpoint to connect to - either the name/path to a
    /// physical device or the IP address of a network to
    /// serial port proxy.
    /// </summary>
    [NotNull, Required]
    [BsonElement("endPoint")]
    public string EndPoint { get; set; } = null!;
}
