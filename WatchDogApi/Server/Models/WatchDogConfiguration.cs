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
    public string? EndPoint { get; set; } = null;

    /// <summary>
    /// Poll Interval in ms
    /// </summary>
    [BsonElement("interval")]
    public int? Interval { get; set; } = null;
}