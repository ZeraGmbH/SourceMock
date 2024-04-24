namespace DutApi.Models;

/// <summary>
/// Configuration for a device under test connection.
/// </summary>
public interface IDeviceUnderTestConnection : IDeviceUnderTest
{
    /// <summary>
    /// Configure the implementation once.
    /// </summary>
    /// <param name="webSamId">Identifier of the device under test.</param>
    /// <param name="connection">Connection configuration to use.</param>
    /// <param name="status">List of status registers.</param>
    /// <returns>Configuration may be asynchronously.</returns>
    public Task Initialize(string webSamId, DutConnection connection, DutStatusRegisterInfo[] status);
}
