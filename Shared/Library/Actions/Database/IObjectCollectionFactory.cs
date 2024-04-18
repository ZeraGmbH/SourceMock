using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
public interface IObjectCollectionFactory<T, TInitializer> where T : IDatabaseObject where TInitializer : ICollectionInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    IObjectCollection<T, TInitializer> Create(string uniqueName, string category);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectCollectionFactory<T> : IObjectCollectionFactory<T, NoopCollectionInitializer<T>> where T : IDatabaseObject
{
}

