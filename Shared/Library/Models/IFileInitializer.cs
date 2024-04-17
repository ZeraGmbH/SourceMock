namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface IFileInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    Task Initialize(IFileCollection<T> collection);
}

/// <summary>
/// 
/// </summary>
public abstract class FileInitializer<T> : IFileInitializer<T>
{
    private readonly object _sync = new();

    private Task? _initializer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    protected abstract Task OnInitialize(IFileCollection<T> collection);

    /// <inheritdoc/>
    public Task Initialize(IFileCollection<T> collection)
    {
        lock (_sync)
            if (_initializer == null)
                _initializer = OnInitialize(collection);

        return _initializer;
    }
}

/// <summary>
/// 
/// </summary>
public class NoopFileInitializer<T> : FileInitializer<T>
{
    /// <inheritdoc/>
    protected override Task OnInitialize(IFileCollection<T> collection) => Task.CompletedTask;
}

