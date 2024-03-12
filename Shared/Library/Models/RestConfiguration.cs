using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models;

/// <summary>
/// Describe a REST endpoint.
/// </summary>
public class RestConfiguration
{
    /// <summary>
    /// Endpoint to use - full URL, e.g. https://demo.zerycon.de/api/v1/Source.
    /// </summary>
    [BsonElement("endpoint")]
    public required string Endpoint { get; set; }
}
