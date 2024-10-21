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
    /// Check if the socket is in an error state.
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Set if the socket is in error - activation or deactivation will reset the flag.</returns>
    Task<bool> GetHasErrorAsync(IInterfaceLogger logger);

    /// <summary>
    /// Set the socket active or inactive..
    /// </summary>
    /// <param name="logger">Interface logging to use.</param>
    /// <param name="active">Set to activate the socket, unset to deactivate it.</param>
    Task SetActiveAsync(bool active, IInterfaceLogger logger);

    /// <summary>
    /// Activate connections for a specific meterform and service type.
    /// </summary>
    /// <param name="meterForm">Meter form as found in the ANSI configuration database.</param>
    /// <param name="serviceType">Service type as found in the ANSI configuration database.</param>
    /// <param name="logger">Interface logging to use.</param>
    Task SetMeterAsync(string meterForm, string serviceType, IInterfaceLogger logger);
}