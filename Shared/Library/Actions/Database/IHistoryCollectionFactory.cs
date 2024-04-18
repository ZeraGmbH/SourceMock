using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
public interface IHistoryCollectionFactory<T, TInitializer> where T : IDatabaseObject where TInitializer : ICollectionInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    IHistoryCollection<T, TInitializer> Create(string uniqueName, string category);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHistoryCollectionFactory<T> : IHistoryCollectionFactory<T, NoopCollectionInitializer<T>> where T : IDatabaseObject
{
}

