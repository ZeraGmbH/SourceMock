namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface ICollectionInitializer<T> where T : IDatabaseObject
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    Task Initialize(IObjectCollection<T> collection);
}

/// <summary>
/// 
/// </summary>
public abstract class CollectionInitializer<T> : ICollectionInitializer<T> where T : IDatabaseObject
{
    private readonly object _sync = new();

    private Task? _initializer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    protected abstract Task OnInitialize(IObjectCollection<T> collection);

    /// <inheritdoc/>
    public Task Initialize(IObjectCollection<T> collection)
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
public class NoopCollectionInitializer<T> : CollectionInitializer<T> where T : IDatabaseObject
{
    /// <inheritdoc/>
    protected override Task OnInitialize(IObjectCollection<T> collection) => Task.CompletedTask;
}

