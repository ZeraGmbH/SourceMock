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
    public string SessionId { get; set; } = null!;

    /// <summary>
    /// Startup time of the logging server.
    /// </summary>
    public DateTime RunIdentifier { get; set; }

    /// <summary>
    /// Relative number of the log entry - starting with zero
    /// when the logging server starts.
    /// </summary>
    public long SequenceCounter { get; set; }

    /// <summary>
    /// The time the log entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
