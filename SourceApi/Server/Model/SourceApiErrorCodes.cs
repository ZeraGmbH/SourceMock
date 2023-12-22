using System.Text.Json.Serialization;

namespace SourceApi.Model;

/// <summary>
/// Lists all errors that may accur in the SourceApi
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SourceApiErrorCodes
{
    /// <summary>
    /// 
    /// </summary>
    SOURCE_NOT_READY
}
