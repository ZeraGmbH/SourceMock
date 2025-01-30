using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing.SerialPort;

/// <summary>
/// Interface to create serial ports - used to make the serial probing testable.
/// </summary>
public interface ISerialPortConnectionForProbing
{
    /// <summary>
    /// Create a new port.
    /// </summary>
    /// <param name="port">Full name of the port.</param>
    /// <param name="options">Some options concerning the physical connection.</param>
    /// <param name="enableReader">Set to activate the background reader.</param>
    /// <returns>A new connection.</returns>
    ISerialPortConnection CreatePhysical(string port, SerialPortOptions? options, bool enableReader);

    /// <summary>
    /// Create a new port.
    /// </summary>
    /// <param name="serverAndPort">Endpoint to the server - must include a port.</param>
    /// <param name="readTimeout">Maximum number of milliseconds to wait for new data.</param>
    /// <param name="enableReader">Set to activate the background reader.</param>
    /// <returns>A new connection.</returns>
    ISerialPortConnection CreateNetwork(string serverAndPort, int? readTimeout, bool enableReader);

}
