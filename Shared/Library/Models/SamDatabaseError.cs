using System.Text.Json.Serialization;

namespace SharedLibrary.Models;

/// <summary>
/// Errors in database
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SamDatabaseError
{
    /// <summary>
    /// item was not found
    /// </summary>
    ITEM_NOT_FOUND,

    /// <summary>
    /// Generic error, if it is not known
    /// </summary>
    DATABASE_ERROR
}
