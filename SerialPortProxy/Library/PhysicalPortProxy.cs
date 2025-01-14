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
    /// Initialiuze a new wrapper.
    /// </summary>
    /// <param name="port">Name of the serial port, e.g. COM3 for Windows
    /// or /dev/ttyUSB0 for a USB serial adapter on Linux.</param>
    /// <param name="options">Additional options.</param>
    public PhysicalPortProxy(string port, SerialPortOptions? options)
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

    /// <inheritdoc/>
    public void Dispose() => _port.Dispose();

    /// <inheritdoc/>
    public byte? RawRead(CancellationToken? cancel)
    {
        var data = _port.ReadByte();

        return data == -1 ? null : checked((byte)data);
    }

    /// <inheritdoc/>
    public void RawWrite(byte[] command, CancellationToken? cancel) => _port.Write(command, 0, command.Length);

    /// <inheritdoc/>
    public string ReadLine(CancellationToken? cancel) => _port.ReadLine();

    /// <inheritdoc/>
    public void WriteLine(string command, CancellationToken? cancel) => _port.WriteLine(command);
}
