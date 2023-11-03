using Amazon.SecurityToken.Model;
using DeviceApiLib.Actions.Database;
using DeviceApiSharedLibrary.Models;
using Microsoft.Extensions.Logging;
using RefMeterApi.Models;

namespace RefMeterApi.Services;

/// <summary>
/// 
/// </summary>
public class DeviceUnderTestStorage : IDeviceUnderTestStorage
{
    private readonly IHistoryCollection<DeviceUnderTest> _collection;

    private readonly ILogger<DeviceUnderTestStorage> _logger;

    /// <summary>
    /// Underlying collection is made available for unit tests.
    /// </summary>
    public IObjectCollection Collection => _collection;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="logger"></param>
    public DeviceUnderTestStorage(IHistoryCollectionFactory<DeviceUnderTest> factory, ILogger<DeviceUnderTestStorage> logger)
    {
        _collection = factory.Create("sam-meter-devices-under-test");
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<DeviceUnderTest> Add(NewDeviceUnderTest device, string user) =>
        _collection.AddItem(new DeviceUnderTest
        {
            Id = Guid.NewGuid().ToString(),
            Name = device.Name
        }, user);

    /// <inheritdoc/>
    public Task<DeviceUnderTest?> Get(string id) =>
        _collection.GetItem(id);

    /// <inheritdoc/>
    public Task<DeviceUnderTest> Update(string id, NewDeviceUnderTest device, string user) =>
        _collection.UpdateItem(new DeviceUnderTest
        {
            Id = id,
            Name = device.Name
        }, user);

    /// <inheritdoc/>
    public Task<DeviceUnderTest> Delete(string id, string user) =>
        _collection.DeleteItem(id, user);

    /// <inheritdoc/>
    public IQueryable<DeviceUnderTest> Query() =>
        _collection.CreateQueryable();
}
