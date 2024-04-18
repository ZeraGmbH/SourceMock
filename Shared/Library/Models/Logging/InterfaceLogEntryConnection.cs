namespace SharedLibrary.Models.Logging;

/// <summary>
/// Describes the connection to a device.
/// </summary>
public class InterfaceLogEntryConnection
{
    /// <summary>
    /// The logical entity generating the entry.
    /// </summary>
    public InterfaceLogSourceTypes WebSamType { get; set; }

    /// <summary>
    /// Unique identifier of the source if there are muliple
    /// instances.
    /// </summary>
    public string WebSamId { get; set; } = null!;

    /// <summary>
    /// Protcol used.
    /// </summary>
    public InterfaceLogProtocolTypes Protocol { get; set; }

    /// <summary>
    /// Unique identifier of the endpoint relative to the protocol.
    /// </summary>
    public string Endpoint { get; set; } = null!;
}
