namespace DutApi.Models;

/// <summary>
/// Access to a device under test.
/// </summary>
public interface IDeviceUnderTest : IDisposable
{
    /// <summary>
    /// Read some status registers.
    /// </summary>
    /// <param name="types">Registers to read.</param>
    /// <returns>Values read</returns>
    Task<object[]> ReadStatusRegisters(DutStatusRegisterTypes[] types);
}
