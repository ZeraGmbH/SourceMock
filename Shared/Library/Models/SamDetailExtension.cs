using System.Text.Json.Serialization;

namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SamDetailExtensions
{
    /// <summary>
    /// 
    /// </summary>
    SamErrorCode,

    /// <summary>
    /// 
    /// </summary>

    SamErrorArgs
}