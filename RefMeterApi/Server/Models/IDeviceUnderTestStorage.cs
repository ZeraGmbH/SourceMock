namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
public interface IDeviceUnderTestStorage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<DeviceUnderTest> Add(NewDeviceUnderTest device, string user);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="device"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<DeviceUnderTest> Update(string id, NewDeviceUnderTest device, string user);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<DeviceUnderTest> Delete(string id, string user);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IQueryable<DeviceUnderTest> Query();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DeviceUnderTest?> Get(string id);
}
