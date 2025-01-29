using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace WatchDogApi.Models;

/// <summary>
/// Configuration of the watchdog device.
/// </summary>
public class WatchDogConfiguration
{
    /// <summary>
    /// Endpoint to the watchdog, e.g. &lt;IP:Port&gt;.
    /// </summary>
    [BsonElement("endPoint")]
    public string? EndPoint { get; set; }

    /// <summary>
    /// Poll Interval in ms
    /// </summary>
    [BsonElement("interval")]
    [Range(1000, 3600 * 1000)]
    public int? Interval { get; set; }
}