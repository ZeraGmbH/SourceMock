using ZERA.WebSam.Shared.Models.Logging;

namespace SerialPortProxy;

/// <summary>
/// A service which is connected through a serial port.
/// </summary>
public interface ISerialPortOwner
{
    /// <summary>
    /// Run commands on a serial port.
    /// </summary>
    /// <param name="requests">Requests to execute.</param>
    /// <param name="logger">Interface logging helper.</param>
    /// <returns>Results of the execution.</returns>
    Task<SerialPortReply[]> ExecuteAsync(IEnumerable<SerialPortCommand> requests, IInterfaceLogger logger);
}
