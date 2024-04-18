namespace SharedLibrary.Models.Logging;

/// <summary>
/// Describe an interface activity.
/// </summary>
public class InterfaceLogEntry : IDatabaseObject
{
    /// <summary>
    /// Unique identifier of the database entry.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Connection to the device.
    /// </summary>
    public InterfaceLogEntryConnection Connection { get; set; } = new();

    /// <summary>
    /// The context of the communication.
    /// </summary>
    public InterfaceLogEntryScope Scope { get; set; } = new();

    /// <summary>
    /// System information on the log entry.
    /// </summary>
    public InterfaceLogEntryInfo Info { get; set; } = new();

    /// <summary>
    /// Payload of the data.
    /// </summary>
    public InterfaceLogPayload Message { get; set; } = new();
}