namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface IFilesCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task RemoveAll();
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFilesCollection<T> : IFilesCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="meta"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    Task<string> AddFile(string name, T meta, Stream content);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteFile(string id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<FileInfo<T>?> FindFile(string id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Stream> Open(string id);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TCommon"></typeparam>
public interface IFilesCollection<T, TCommon> : IFilesCollection<T>
{
    /// <summary>
    /// 
    /// </summary>
    TCommon Common { get; }
}
