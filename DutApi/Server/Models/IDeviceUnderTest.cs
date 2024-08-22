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
    /// <param name="logger">Interface logging sink.</param>
    /// <param name="types">Registers to read.</param>
    /// <param name="cancel">Test if operation should be cancelled.</param>
    /// <returns>Values read</returns>
    Task<object[]> ReadStatusRegisters(IInterfaceLogger logger, DutStatusRegisterTypes[] types, CancellationToken? cancel = null);

    /// <summary>
    /// Execute a single command string.
    /// </summary>
    /// <param name="logger">Interface logging sink.</param>
    /// <param name="command">Command to send.</param>
    /// <param name="responseLines">Number of reply lines expected.</param>
    /// <param name="cancel">Test if operation should be cancelled.</param>
    /// <returns>Raw data received from the device.</returns>
    Task<string[]> DirectCommand(IInterfaceLogger logger, string command, int responseLines, CancellationToken? cancel = null);
}
