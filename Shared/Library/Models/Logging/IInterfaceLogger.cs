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
}
