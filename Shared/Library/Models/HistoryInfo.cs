using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models;

/// <summary>
/// Additional data added to each document.
/// </summary>
public class HistoryInfo
{
    /// <summary>
    /// Exact time when the item was added to the database.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who added the item to the database.
    /// </summary>
    [BsonElement("createdBy")]
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Exact time when the item was last modified.
    /// </summary>
    [BsonElement("modifiedAt")]
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// User who saved the last modification of the document.
    /// </summary>
    [BsonElement("modifiedBy")]
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// Number of times the document was changed - including the first add.
    /// </summary>
    [BsonElement("version")]
    public long ChangeCount { get; set; }
}

