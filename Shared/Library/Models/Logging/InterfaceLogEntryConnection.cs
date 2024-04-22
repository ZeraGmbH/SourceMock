namespace SharedLibrary.Models.Logging;

/// <summary>
/// Describes the connection to a device.
/// </summary>
public class InterfaceLogEntryTargetConnection
{
    /// <summary>
    /// Protcol used.
    /// </summary>
    public required InterfaceLogProtocolTypes Protocol { get; set; }

    /// <summary>
    /// Unique identifier of the endpoint relative to the protocol.
    /// </summary>
    public required string Endpoint { get; set; }
}

/// <summary>
/// Describes the connection to a device.
/// </summary>
public class InterfaceLogEntryConnection : InterfaceLogEntryTargetConnection
{
    /// <summary>
    /// The logical entity generating the entry.
    /// </summary>
    public required InterfaceLogSourceTypes WebSamType { get; set; }

    /// <summary>
    /// Unique identifier of the source if there are muliple
    /// instances.
    /// </summary>
    public string WebSamId { get; set; } = null!;
}
