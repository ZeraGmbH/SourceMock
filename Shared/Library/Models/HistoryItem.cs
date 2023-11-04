using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceApiSharedLibrary.Models;

/// <summary>
/// Describes a document including a detailed version information.
/// </summary>
/// <typeparam name="T">The concrete type of the document.</typeparam>
public class HistoryItem<T> where T : IDatabaseObject
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
    /// The document in the indicated version.
    /// </summary>
    public T Item { get; set; } = default!;

    /// <summary>
    /// Detailed change information for the document - including the version
    /// number which is 1-based.
    /// </summary>
    public HistoryInfo Version { get; set; } = default!;
}
