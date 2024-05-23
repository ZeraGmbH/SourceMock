using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models.Logging;

/// <summary>
/// Global configuration of the logging system.
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// Number of days to keep interface log entries which do not
    /// belong to unsaved sessions. 
    /// </summary>
    [NotNull, Required]
    [BsonElement("interfaceLogRetentionDays")]
    public int UnsavedInterfaceLogRetentionDays { get; set; } = 30;
}
