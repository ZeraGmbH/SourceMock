namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class FileInfo<TItem>
{
    /// <summary>
    /// 
    /// </summary>
    public required TItem Meta { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required long Length { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required DateTime UploadedAt { get; set; }
}

/// <summary>
/// 
/// </summary>
public interface IFileCollection
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
public interface IFileCollection<T> : IFileCollection
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
public interface IFileCollection<T, TCommon> : IFileCollection<T>
{
    /// <summary>
    /// 
    /// </summary>
    TCommon Common { get; }
}
