using SharedLibrary.Models.Logging;

namespace SharedLibrary.Actions;

/// <summary>
/// 
/// </summary>
public class NoopInterfaceLogger : IInterfaceLogger
{
    private class PreparedEntry : IPreparedInterfaceLogEntry
    {
        /// <inheritdoc/>
        public InterfaceLogEntry? Finish(InterfaceLogPayload payload) => null!;

        /// <inheritdoc/>
        public Task<InterfaceLogEntry?> FinishAsync(InterfaceLogPayload payload) => Task.FromResult<InterfaceLogEntry?>(null);
    }

    private class Connection : IInterfaceConnection
    {
        /// <inheritdoc/>
        public IPreparedInterfaceLogEntry Prepare(InterfaceLogEntryScope scope) => new PreparedEntry();
    }

    /// <inheritdoc/>
    public IInterfaceConnection CreateConnection(InterfaceLogEntryConnection connection) => new Connection();

    /// <inheritdoc/>
    public IQueryable<InterfaceLogEntry> Query() => Array.Empty<InterfaceLogEntry>().AsQueryable();
}
