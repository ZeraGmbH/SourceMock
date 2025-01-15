using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace ZIFApi.Models;

/// <summary>
/// Interface provided by all ZIF implementations.
/// </summary>
public interface IZIFProtocol
{
    /// <summary>
    /// The test position of the socket.
    /// </summary>
    int Index { get; set; }

    /// <summary>
    /// Optional read timeout in milliseconds.
    /// </summary>
    int? ReadTimeout { get; set; }

    /// <summary>
    /// Read the software version.
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Version information.</returns>
    Task<ZIFVersionInfo> GetVersionAsync(ISerialPortConnection factory, IInterfaceLogger logger);

    /// <summary>
    /// Retrieve the serial number.
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Serial number.</returns>
    Task<int> GetSerialAsync(ISerialPortConnection factory, IInterfaceLogger logger);

    /// <summary>
    /// See if the socket is active.
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Set if the socket is active.</returns>
    Task<bool> GetActiveAsync(ISerialPortConnection factory, IInterfaceLogger logger);

    /// <summary>
    /// See if the socket is active with a meter placed on it.
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Set if the socket is active with a meter placed on it.</returns>
    Task<bool> GetHasMeterAsync(ISerialPortConnection factory, IInterfaceLogger logger);

    /// <summary>
    /// Check if the socket is in an error state.
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <returns>Set if the socket is in error - activation or deactivation will reset the flag.</returns>
    Task<bool> GetHasErrorAsync(ISerialPortConnection factory, IInterfaceLogger logger);

    /// <summary>
    /// Set the socket active or inactive..
    /// </summary>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    /// <param name="active">Set to activate the socket, unset to deactivate it.</param>
    Task SetActiveAsync(bool active, ISerialPortConnection factory, IInterfaceLogger logger);

    /// <summary>
    /// Activate connections for a specific meterform and service type.
    /// </summary>
    /// <param name="meterForm">Meter form as found in the ANSI configuration database.</param>
    /// <param name="serviceType">Service type as found in the ANSI configuration database.</param>
    /// <param name="factory">Serial port factory to use.</param>
    /// <param name="logger">Interface logging to use.</param>
    Task SetMeterAsync(string meterForm, string serviceType, ISerialPortConnection factory, IInterfaceLogger logger);
}