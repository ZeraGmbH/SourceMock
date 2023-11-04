using DeviceApiSharedLibrary.Models;

namespace DeviceApiLib.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectCollectionFactory<T> where T : IDatabaseObject
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueName"></param>
    /// <returns></returns>
    IObjectCollection<T> Create(string uniqueName);
}
