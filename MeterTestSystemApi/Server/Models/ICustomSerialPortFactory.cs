using MeterTestSystemApi.Models.Configuration;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Models;

/// <summary>
/// 
/// /// </summary>
public interface ICustomSerialPortFactory : IDisposable
{
    /// <summary>
    /// Setup a configured custom serial ports.
    /// </summary>
    /// <param name="ports">List of serial ports.</param>
    void Initialize(Dictionary<string, CustomSerialPortConfiguration> ports);

    /// <summary>
    /// Run commands on a serial port.
    /// </summary>
    /// <param name="name">Name of the serial port.</param>
    /// <param name="requests">Requests to execute.</param>
    /// <param name="logger">Interface logging helper.</param>
    /// <returns>Results of the execution.</returns>
    Task<SerialPortReply[]> ExecuteAsync(string name, IEnumerable<SerialPortCommand> requests, IInterfaceLogger logger);
}