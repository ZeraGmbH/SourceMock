namespace SerialPortProxy;

/// <summary>
/// Share some response as long as the next request arrives while still executing.
/// </summary>
public class ResponseShare<T>
{
    /// <summary>
    /// Synchronize access to _task;
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// The current executing request if any.
    /// </summary>
    private Task<T>? _task;

    /// <summary>
    /// Method to create a new request task.
    /// </summary>
    private readonly Func<Task<T>> _factory;

    /// <summary>
    /// Initialize zhe response share instance.
    /// </summary>
    /// <param name="factory">Method to create a new request task.</param>
    public ResponseShare(Func<Task<T>> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Create a new request task and reset cache when done.
    /// </summary>
    /// <returns>The new task.</returns>
    private async Task<T> CreateTask()
    {
        try
        {
            /* Must await to make finally work correctly. */
            return await _factory();
        }
        finally
        {
            lock (_lock)
                _task = null;
        }
    }

    /// <summary>
    /// Execute some request.
    /// </summary>
    /// <returns>A task related with the request.</returns>
    public Task<T> Execute()
    {
        /* Make sure there is only one request running. */
        lock (_lock)
            return _task ??= CreateTask();
    }
}
