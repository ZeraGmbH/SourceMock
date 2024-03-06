using MongoDB.Bson.Serialization.Attributes;

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
    public required ErrorCalculatorProtocols Protocol { get; set; }

    /// <summary>
    /// Type of connection to use.
    /// </summary>
    [BsonElement("connection")]
    public required ErrorCalculatorConnectionTypes Connection { get; set; }

    /// <summary>
    /// Depending on the connection type a description of the endpoint
    /// to connect to - e.g. IP:Port for TCP base connections.
    /// </summary>
    [BsonElement("endpoint")]
    public required string EndPoint { get; set; }
}