using System.IO.Ports;

namespace SerialPortProxy;

/// <summary>
/// Physical serial port mapped to the ISerialPort interface.
/// </summary>
public class PhysicalPortProxy : ISerialPort
{
    /// <summary>
    /// The physical connection.
    /// </summary>
    private readonly SerialPort _port;

    /// <summary>
    /// All ports currently open.
    /// </summary>
    private static readonly HashSet<string> _openPorts = [];

    /// <summary>
    /// The key of this port.
    /// </summary>
    private readonly string? _portKey;

    /// <summary>
    /// Initialiuze a new wrapper.
    /// </summary>
    /// <param name="port">Name of the serial port, e.g. COM3 for Windows
    /// or /dev/ttyUSB0 for a USB serial adapter on Linux.</param>
    /// <param name="options">Additional options.</param>
    public PhysicalPortProxy(string port, SerialPortOptions? options)
    {
        /* Make sure port can be opened only once. */
        lock (_openPorts)
            if (!_openPorts.Add(port))
                throw new InvalidOperationException($"serial port {port} already in use");

        /* Now remember or key. */
        _portKey = port;

        try
        {
            /* Connected as required in the API documentation. MT3101_RS232_EXT_GB.pdf */
            _port = new SerialPort
            {
                BaudRate = options?.BaudRate ?? 9600,
                DataBits = options?.DataBits ?? 8,
                NewLine = options?.NewLine ?? "\r",
                Parity = (Parity?)options?.Parity ?? Parity.None,
                PortName = port,
                ReadTimeout = options?.ReadTimeout ?? 30000,
                StopBits = (StopBits?)options?.StopBits ?? StopBits.Two,
                WriteTimeout = options?.WriteTimeout ?? 30000
            };

            /* Always open the connection immediatly. */
            _port.Open();
        }
        catch (Exception)
        {
            lock (_openPorts)
                _openPorts.Remove(_portKey);

            throw;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        try
        {
            _port.Dispose();
        }
        finally
        {
            if (!string.IsNullOrEmpty(_portKey))
                lock (_openPorts)
                    _openPorts.Remove(_portKey);
        }
    }

    /// <inheritdoc/>
    public byte? RawRead(int? timeout = null)
    {
        /* Busy wait while no data is available. */
        if (timeout != null)
            for (var end = DateTime.UtcNow.AddMilliseconds(timeout.Value); _port.IsOpen && _port.BytesToRead < 1; Thread.Sleep(10))
                if (DateTime.UtcNow >= end)
                    throw new TimeoutException("read operation timed out");

        /* Blocked read of next byte. */
        var data = _port.ReadByte();

        return data == -1 ? null : checked((byte)data);
    }

    /// <inheritdoc/>
    public void RawWrite(byte[] command) => _port.Write(command, 0, command.Length);

    /// <inheritdoc/>
    public string ReadLine() => _port.ReadLine();

    /// <inheritdoc/>
    public void WriteLine(string command) => _port.WriteLine(command);
}
