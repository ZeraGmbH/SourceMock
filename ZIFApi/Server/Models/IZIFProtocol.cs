using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace ZIFApi.Models;

/// <summary>
/// Interface provided by all ZIF implementations.
/// </summary>
public interface IZIFProtocol
{
    /// <summary>
    /// Read the software version.
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Version information.</returns>
    Task<VersionInfo> GetVersion(ISerialPortConnection factory, IInterfaceLogger logger);
}