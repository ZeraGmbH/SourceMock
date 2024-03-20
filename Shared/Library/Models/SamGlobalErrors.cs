using System.Text.Json.Serialization;

namespace SharedLibrary.Models;

/// <summary>
/// Errors that can occur unpredictable
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SamGlobalErrors
{
    /// <summary>
    /// 
    /// </summary>
    GENERAL_ERROR = 0
}
