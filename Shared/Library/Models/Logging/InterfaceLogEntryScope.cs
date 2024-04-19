namespace SharedLibrary.Models.Logging;

/// <summary>
/// The logical context of a communication event.s
/// </summary>
public class InterfaceLogEntryScope
{
    /// <summary>
    /// Combines a group of operations together.
    /// </summary>
    public string CorrelationId { get; set; } = null!;

    /// <summary>
    /// Allows to identify all response for a single request.
    /// </summary>
    public string RequestId { get; set; } = null!;

    /// <summary>
    /// Set for sent data, unset for incoming.
    /// </summary>
    public required bool Outgoing { get; set; }
}
