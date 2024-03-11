using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutApi.Models;

/// <summary>
/// Describe the physical connection to a device under test.
/// </summary>
public class DutConnection
{
    /// <summary>
    /// Protocol to use
    /// </summary>
    [BsonElement("protocol"), Required, NotNull]
    public DutProtocolTypes Type { get; set; }

    /// <summary>
    /// Endpoint to connect to - format depends on the protocol.
    /// </summary>
    [BsonElement("endpoint"), Required, NotNull, MinLength(1)]
    public string? Endpoint { get; set; }
}
