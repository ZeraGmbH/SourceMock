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
    public required string SessionId { get; set; }

    /// <summary>
    /// Startup time of the logging server.
    /// </summary>
    public required DateTime RunIdentifier { get; set; }

    /// <summary>
    /// Combines a group of operations together.
    /// </summary>
    public required string CorrelationId { get; set; }

    /// <summary>
    /// Relative number of the log entry - starting with zero
    /// when the logging server starts.
    /// </summary>
    public required long SequenceCounter { get; set; }

    /// <summary>
    /// The time the log entry was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}
