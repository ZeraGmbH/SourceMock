using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;
using ZeraDevices.ErrorCalculator.STM;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// Configuration of a single connection to an
/// error calculator.
/// </summary>
public class ErrorCalculatorConfiguration
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    [BsonElement("protocol")]
    [NotNull, Required]
    public required ErrorCalculatorProtocols Protocol { get; set; }

    /// <summary>
    /// Type of connection to use.
    /// </summary>
    [BsonElement("connection")]
    [NotNull, Required]
    public required ErrorCalculatorConnectionTypes Connection { get; set; }

    /// <summary>
    /// Depending on the connection type a description of the endpoint
    /// to connect to - e.g. "http://IP:Port/" for TCP based connections using the
    /// MAD XML protocol oder full endpoint when using the HTTP proxy.
    /// </summary>
    [BsonElement("endpoint")]
    [NotNull, Required]
    public required string Endpoint { get; set; }
}