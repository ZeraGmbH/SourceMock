namespace SharedLibrary.Models.Logging;

/// <summary>
/// Describe an interface activity.
/// </summary>
public class InterfaceLogEntry : IDatabaseObject
{
    /// <summary>
    /// Unique identifier of the database entry.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Connection to the device.
    /// </summary>
    public required InterfaceLogEntryConnection Connection { get; set; }

    /// <summary>
    /// The context of the communication.
    /// </summary>
    public required InterfaceLogEntryScope Scope { get; set; }

    /// <summary>
    /// System information on the log entry.
    /// </summary>
    public required InterfaceLogEntryInfo Info { get; set; }

    /// <summary>
    /// Payload of the data.
    /// </summary>
    public InterfaceLogPayload Message { get; set; } = null!;
}