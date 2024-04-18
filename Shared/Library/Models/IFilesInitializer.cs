namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface IFilesInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    Task Initialize(IFilesCollection<T> collection);
}

/// <summary>
/// 
/// </summary>
public abstract class FilesInitializer<T> : IFilesInitializer<T>
{
    private readonly object _sync = new();

    private Task? _initializer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    protected abstract Task OnInitialize(IFilesCollection<T> collection);

    /// <inheritdoc/>
    public Task Initialize(IFilesCollection<T> collection)
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
public class NoopFilesInitializer<T> : FilesInitializer<T>
{
    /// <inheritdoc/>
    protected override Task OnInitialize(IFilesCollection<T> collection) => Task.CompletedTask;
}

