using SharedLibrary.Models.Logging;

/// <summary>
/// 
/// </summary>
public class NoopInterfaceLogger : IInterfaceLogger
{
    private class PreparedEntry : IPreparedInterfaceLogEntry
    {
        /// <inheritdoc/>
        public void Finish(InterfaceLogPayload payload)
        {
        }
    }

    private class Connection : IInterfaceConnection
    {
        /// <inheritdoc/>
        public IPreparedInterfaceLogEntry Prepare(InterfaceLogEntryScope scope) => new PreparedEntry();
    }

    /// <inheritdoc/>
    public IInterfaceConnection CreateConnection(InterfaceLogEntryConnection connection) => new Connection();
}
