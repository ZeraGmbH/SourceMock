using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// Manage counters,
/// </summary>
public sealed class InMemoryCounters : ICounterCollection
{
    private readonly Dictionary<string, long> _counters = [];

    /// <inheritdoc />
    public Task<long> GetNextCounter(string name)
    {
        lock (_counters)
        {
            /* Read current counter. */
            if (!_counters.TryGetValue(name, out var counter)) counter = 0;

            /* Increment - first value would be 1. */
            ++counter;

            _counters[name] = counter;

            return Task.FromResult(counter);
        }
    }
}

/// <summary>
/// 
/// </summary>
public class InMemoryCountersFactory : ICounterCollectionFactory
{
    /// <inheritdoc/>
    public ICounterCollection Create(string category) => new InMemoryCounters();
}

