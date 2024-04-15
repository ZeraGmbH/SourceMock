using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSingleton"></typeparam>
public interface IObjectCollectionFactory<T, TSingleton> where T : IDatabaseObject where TSingleton : ICollectionInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    IObjectCollection<T, TSingleton> Create(string uniqueName, string category);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectCollectionFactory<T> : IObjectCollectionFactory<T, NoopInitializer<T>> where T : IDatabaseObject
{
}

