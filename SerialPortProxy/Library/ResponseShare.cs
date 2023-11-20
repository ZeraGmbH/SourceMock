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
    /// For testing purposes see if task is active.
    /// </summary>
    public bool IsBusy => _task != null;

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
            /*
                We have to process the reset of the active task
                to avoid a possible race condition. In case the
                Task created by the factory executes synchronously
                (i.e. immediate on the current thread) the lock
                is still hold by the caller (Execute). In this
                situation the active task is reset here - which 
                does nothing. In the next step Execute sets the
                active task to the already completed task which
                will never be released in the future.
            */
            ThreadPool.QueueUserWorkItem((state) =>
            {
                lock (_lock)
                    _task = null;
            });
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
