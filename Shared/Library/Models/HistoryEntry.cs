using MongoDB.Bson.Serialization.Attributes;

namespace DeviceApiSharedLibrary.Models;

/// <summary>
/// Some document in the history collection.
/// </summary>
/// <typeparam name="T">The type of each historized document.</typeparam>
public class HistoryEntry<T> : DatabaseObject where T : DatabaseObject
{
    /// <summary>
    /// The original item.
    /// </summary>
    [BsonElement("item")]
    public T Item { get; set; } = default!;
}

