namespace DutApi.Models;

/// <summary>
/// Create a device under test implementation
/// </summary>
public interface IDeviceUnderTestFactory
{
    /// <summary>
    /// Based on the connection data create a brand new device
    /// under test accessor.
    /// </summary>
    /// <param name="webSamId">Identifier of the device under test.</param>
    /// <param name="connection">Connection to use.</param>
    /// <param name="status">List of status registers.</param>
    /// <returns>A new device under test.</returns>
    Task<IDeviceUnderTest> Create(string webSamId, DutConnection connection, DutStatusRegisterInfo[] status);
}
