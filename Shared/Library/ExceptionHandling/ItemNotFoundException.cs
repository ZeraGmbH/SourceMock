namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="id"></param>
public class ItemNotFoundException(string id) : Exception($"Item with id '{id}' cannot be found in database")
{
    /// <summary>
    /// 
    /// </summary>
    public readonly string ItemId = id;
}