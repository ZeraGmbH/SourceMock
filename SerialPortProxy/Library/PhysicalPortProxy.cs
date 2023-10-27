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
    public PhysicalPortProxy(string port)
    {
        /* Connected as required in the API documentation. MT3101_RS232_EXT_GB.pdf */
        _port = new SerialPort
        {
            BaudRate = 9600,
            DataBits = 8,
            NewLine = "\r",
            Parity = Parity.None,
            PortName = port,
            ReadTimeout = 30000,
            StopBits = StopBits.Two,
            WriteTimeout = 30000
        };

        /* Always open the connection immediatly. */
        _port.Open();
    }

    /// <inheritdoc/>
    public void Dispose() => _port.Dispose();

    /// <inheritdoc/>
    public string ReadLine() => _port.ReadLine();

    /// <inheritdoc/>
    public void WriteLine(string command) => _port.WriteLine(command);
}
