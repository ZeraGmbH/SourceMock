using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
public interface IFileCollectionFactory<T, TInitializer> where TInitializer : IFileInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    IFileCollection<T, TInitializer> Create(string uniqueName, string category);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFileCollectionFactory<T> : IFileCollectionFactory<T, NoopFileInitializer<T>>
{
}

