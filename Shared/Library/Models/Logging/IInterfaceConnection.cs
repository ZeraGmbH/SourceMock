namespace SharedLibrary.Models.Logging;

/// <summary>
/// Represents a single device connection.
/// </summary>
public interface IInterfaceConnection
{
    /// <summary>
    /// Start a log entry.
    /// </summary>
    /// <param name="scope">Describes the current operation.</param>
    /// <returns>Service to finish the operation.</returns>
    IPreparedInterfaceLogEntry Prepare(InterfaceLogEntryScope scope);
}
