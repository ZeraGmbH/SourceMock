using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharedLibrary.Models.Logging;

/// <summary>
/// System information on the log entry.
/// </summary>
public class InterfaceLogEntryInfo
{
    /// <summary>
    /// The session initiating the activity. Using this information 
    /// the responsible user can be detected.
    /// </summary>
    [NotNull, Required]
    public required string SessionId { get; set; }

    /// <summary>
    /// Startup time of the logging server.
    /// </summary>
    [NotNull, Required]
    public required DateTime RunIdentifier { get; set; }

    /// <summary>
    /// Combines a group of operations together.
    /// </summary>
    [NotNull, Required]
    public required string CorrelationId { get; set; }

    /// <summary>
    /// Relative number of the log entry - starting with zero
    /// when the logging server starts.
    /// </summary>
    [NotNull, Required]
    public required long SequenceCounter { get; set; }

    /// <summary>
    /// The time the log entry was created.
    /// </summary>
    [NotNull, Required]
    public required DateTime CreatedAt { get; set; }
}
