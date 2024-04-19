using SharedLibrary.Models.Logging;

/// <summary>
/// 
/// </summary>
public class NoopInterfaceLogger : IInterfaceLogger
{
    private class PreparedEntry : IPreparedInterfaceLogEntry
    {
        /// <inheritdoc/>
        public InterfaceLogEntry? Finish(InterfaceLogPayload payload) => null!;
    }

    private class Connection : IInterfaceConnection
    {
        /// <inheritdoc/>
        public IPreparedInterfaceLogEntry Prepare(InterfaceLogEntryScope scope) => new PreparedEntry();
    }

    /// <inheritdoc/>
    public IInterfaceConnection CreateConnection(InterfaceLogEntryConnection connection) => new Connection();

    public IQueryable<InterfaceLogEntry> Query() => Array.Empty<InterfaceLogEntry>().AsQueryable();
}
