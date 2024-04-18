namespace SharedLibrary.Models.Logging;

/// <summary>
/// Pending log entry - to keep sequence request entries will
/// be created prior to sending the request to the device.
/// </summary>
public interface IPreparedInterfaceLogEntry
{
    /// <summary>
    /// Create a log entry after a communication completed -
    /// eventuall including some error information.
    /// </summary>
    /// <param name="payload">Payload data.</param>
    void Finish(InterfaceLogPayload payload);
}
