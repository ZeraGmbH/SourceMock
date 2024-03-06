using MongoDB.Bson.Serialization.Attributes;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Describe a REST endpoint.
/// </summary>
public class RestConfiguration
{
    /// <summary>
    /// Endpoint to use - full URL, e.g. https://demo.zerycon.de/api/v1/Source.
    /// </summary>
    [BsonElement("endpoint")]
    public required string EndPoint { get; set; }
}
