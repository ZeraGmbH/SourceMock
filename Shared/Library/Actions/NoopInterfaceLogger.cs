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
    public event Action<InterfaceLogEntry> EntryAdded = null!;

    /// <inheritdoc/>
    public IInterfaceConnection CreateConnection(InterfaceLogEntryConnection connection) => new Connection();

    /// <inheritdoc/>
    public IQueryable<InterfaceLogEntry> Query() => Array.Empty<InterfaceLogEntry>().AsQueryable();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    public void Fire(InterfaceLogEntry entry) => EntryAdded?.Invoke(entry);

    /// <inheritdoc/>
    public Task Delete(string sessionId) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task<Stream> Export(string sessionId) => Task.FromResult<Stream>(new MemoryStream());
}
