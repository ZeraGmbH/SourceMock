using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Default class to create serial ports for probing.
/// </summary>
public class SerialPortConnectionForProbing(ILogger<SerialPortConnection> logger) : ISerialPortConnectionForProbing
{
    /// <inheritdoc/>
    public ISerialPortConnection CreatePhysical(string port, SerialPortOptions? options, bool enableReader)
        => SerialPortConnection.FromSerialPort(port, options, logger, enableReader);

    /// <inheritdoc/>
    public ISerialPortConnection CreateNetwork(string serverAndPort, int? readTimeout, bool enableReader)
        => SerialPortConnection.FromNetwork(serverAndPort, logger, enableReader, readTimeout);
}
