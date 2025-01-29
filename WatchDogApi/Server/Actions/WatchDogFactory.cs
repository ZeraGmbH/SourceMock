using WatchDogApi.Models;

namespace WatchDogApi.Actions;

/// <summary>
/// Implement the factory to create watchdogs.
/// </summary>
public class WatchDogFactory(IServiceProvider services, IWatchDogExecuter watchDogExecuter) : IWatchDogFactory
{

    private readonly object _sync = new();

    private bool _initialized = false;

    private WatchDog _watchDog = null!;

    /// <inheritdoc/>
    public IWatchDog WatchDog
    {
        get
        {
            lock (_sync)
            {
                while (!_initialized)
                    Monitor.Wait(_sync);

                return _watchDog;
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_sync)
        {
            _initialized = true;

            _watchDog?.Terminate();

            Monitor.PulseAll(_sync);
        }
    }

    /// <inheritdoc/>
    public void Initialize(WatchDogConfiguration config)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_initialized) throw new InvalidOperationException("WatchDog already initialized");

            try
            {
                /* No watchdog active. */
                _watchDog = new WatchDog(config, services, watchDogExecuter);
            }
            finally
            {
                /* Use the new instance. */
                _initialized = true;

                /* Signal availability of meter test system. */
                Monitor.PulseAll(_sync);
            }
        }
    }
}