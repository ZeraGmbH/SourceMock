using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
public interface IFilesCollectionFactory<T, TInitializer> where TInitializer : IFilesInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    IFilesCollection<T, TInitializer> Create(string uniqueName, string category);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFilesCollectionFactory<T> : IFilesCollectionFactory<T, NoopFilesInitializer<T>>
{
}

