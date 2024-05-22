namespace SharedLibrary.Models.Logging;

/// <summary>
/// Interface provided by the interface logging service.
/// </summary>
public interface IInterfaceLogger
{
    /// <summary>
    /// Define the connection to a device.
    /// </summary>
    /// <param name="connection">Description of a connection.</param>
    /// <returns>Service to create log entries.</returns>
    IInterfaceConnection CreateConnection(InterfaceLogEntryConnection connection);

    /// <summary>
    /// Query interface log entries.
    /// </summary>
    /// <returns>Query builder.</returns>
    IQueryable<InterfaceLogEntry> Query();

    /// <summary>
    /// Will fire whenever a new log entry has been created.
    /// </summary>
    event Action<InterfaceLogEntry> EntryAdded;

    /// <summary>
    /// Delete all entries related to a session.
    /// </summary>
    /// <param name="sessionId">The session to delete entries for.</param>
    Task Delete(string sessionId);

    /// <summary>
    /// Export all log entries of a single session as a JSON array.
    /// </summary>
    /// <param name="sessionId">Session to use.</param>
    /// <returns>UTF-8 byte stream with JSON array contents.</returns>
    Task<Stream> Export(string sessionId);
}
