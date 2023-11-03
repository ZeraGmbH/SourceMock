using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceApiSharedLibrary.Models;

/// <summary>
/// Represents some database object with a required unique
/// identifier.
/// </summary>
public class DatabaseObject
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
}

