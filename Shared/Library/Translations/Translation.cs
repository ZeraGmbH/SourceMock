using MongoDB.Bson.Serialization.Attributes;
using SharedLibrary.Models;

namespace SharedLibrary.Translations;

/// <summary>
/// 
/// </summary>
public class Translation : IDatabaseObject
{
    /// <summary>
    /// language + key
    /// </summary>
    [BsonId]
    public required string Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public required string Language { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public required string Key { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public required string Text { get; set; }
}