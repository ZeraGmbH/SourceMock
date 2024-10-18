using ZERA.WebSam.Shared.Models.Logging;

namespace ZIFApi.Models;

/// <summary>
/// 
/// </summary>
public interface IZIFDevice
{
    /// <summary>
    /// Read the software version.
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Version information.</returns>
    Task<ZIFVersionInfo> GetVersion(IInterfaceLogger logger);
}