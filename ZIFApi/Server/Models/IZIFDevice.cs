using ZERA.WebSam.Shared.Actions;
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
    Task<ZIFVersionInfo> GetVersionAsync(IInterfaceLogger logger);

    /// <summary>
    /// Retrieve the serial number.
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Serial number.</returns>
    Task<int> GetSerialAsync(IInterfaceLogger logger);

    /// <summary>
    /// See if the socket is active.
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Set if the socket is active.</returns>
    Task<bool> GetActiveAsync(IInterfaceLogger logger);

    /// <summary>
    /// See if the socket is active with a meter placed on it.
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Set if the socket is active with a meter placed on it.</returns>
    Task<bool> GetHasMeterAsync(IInterfaceLogger logger);

    /// <summary>
    /// Set the socket active or inactive..
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <param name="active">Set to activate the socket, unset to deactivate it.</param>
    Task SetActiveAsync(bool active, IInterfaceLogger logger);
}