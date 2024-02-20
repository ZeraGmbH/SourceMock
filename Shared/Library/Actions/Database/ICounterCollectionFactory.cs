using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
public interface ICounterCollectionFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    ICounterCollection Create();
}
