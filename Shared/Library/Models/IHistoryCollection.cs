namespace SharedLibrary.Models;

/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
public interface IHistoryCollection<T> : IBasicObjectCollection<T> where T : IDatabaseObject
{
    /// <summary>
    /// Get the history of a single entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>History informnation of the item sorted by version descending - i.e. newest first.</returns>
    Task<IEnumerable<HistoryInfo>> GetHistory(string id);

    /// <summary>
    /// Retrieve a specific version of some entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="version">The version number to look up - starting with 1.</param>
    /// <returns>The item in the indicated version</returns>
    Task<T> GetHistoryItem(string id, long version);
}


/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
/// <typeparam name="TCommon"></typeparam>
public interface IHistoryCollection<T, TCommon> : IHistoryCollection<T> where T : IDatabaseObject where TCommon : ICollectionInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    TCommon Common { get; }
}