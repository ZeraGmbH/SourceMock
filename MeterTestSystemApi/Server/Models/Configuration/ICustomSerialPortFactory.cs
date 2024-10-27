using SerialPortProxy;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// 
/// /// </summary>
public interface ICustomSerialPortFactory : IDisposable
{
    /// <summary>
    /// Method to access a single serial port.
    /// </summary>
    Func<string, ISerialPortConnection> GetCustomPort { get; }

    /// <summary>
    /// Setup a configured custom serial ports.
    /// </summary>
    /// <param name="ports">List of serial ports.</param>
    void Initialize(Dictionary<string, CustomSerialPortConfiguration> ports);
}