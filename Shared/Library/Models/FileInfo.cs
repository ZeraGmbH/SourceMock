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

    /// <summary>
    /// 
    /// </summary>
    public required string UserId { get; set; }

}
