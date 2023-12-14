using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models;

/// <summary>
/// Some document in the history collection.
/// </summary>
/// <typeparam name="T">The type of each historized document.</typeparam>
public class HistoryEntry<T> : IDatabaseObject where T : IDatabaseObject
{
    /// <summary>
    /// Unique identifer of the object which can be used
    /// as a primary key. Defaults to a new Guid.
    /// </summary>
    [BsonId]
    [Required]
    [NotNull]
    [MinLength(1)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The original item.
    /// </summary>
    [BsonElement("item")]
    public T Item { get; set; } = default!;
}

