namespace SharedLibrary.Models;

/// <summary>
/// Errors in database
/// </summary>
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
