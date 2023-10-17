namespace SerialPortProxy;

/// <summary>
/// Representation of a single serial port connection. It's strongly recommended
/// to use only singletons.
/// </summary>
public class SerialPortService : IDisposable
{
    private readonly SerialPortConnection _connection;

    /// <summary>
    /// Initialize a serial port service instance.
    /// </summary>
    /// <param name="configuration">Configuration of the port.</param>
    public SerialPortService(SerialPortConfiguration configuration)
    {
        if (configuration.UseMockType)
            _connection = SerialPortConnection.FromMock(Type.GetType(configuration.PortNameOrMockType)!);
        else
            _connection = SerialPortConnection.FromSerialPort(configuration.PortNameOrMockType);
    }

    /// <inheritdoc/>
    public void Dispose() => _connection.Dispose();

    /// <inheritdoc/>
    public Task<string[]>[] Execute(params SerialPortRequest[] requests) => _connection.Execute(requests);
}