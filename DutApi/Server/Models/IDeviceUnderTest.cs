using ZERA.WebSam.Shared.Models.Logging;

namespace DutApi.Models;

/// <summary>
/// Access to a device under test.
/// </summary>
public interface IDeviceUnderTest : IDisposable
{
    /// <summary>
    /// Read some status registers.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="types">Registers to read.</param>
    /// <param name="cancel">Test if operation should be cancelled.</param>
    /// <returns>Values read</returns>
    Task<object[]> ReadStatusRegisters(IInterfaceLogger logger, DutStatusRegisterTypes[] types, CancellationToken? cancel = null);
}
