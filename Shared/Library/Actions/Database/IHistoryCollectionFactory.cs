using DeviceApiSharedLibrary.Models;

namespace DeviceApiLib.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHistoryCollectionFactory<T> where T : IDatabaseObject
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <returns></returns>
    IHistoryCollection<T> Create(string uniqueName);
}
