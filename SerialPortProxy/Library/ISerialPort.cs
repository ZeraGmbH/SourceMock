namespace SerialPortProxy;

/// <summary>
/// Represents the minimal set of methods neccessary to communicate with a serial port.
/// </summary>
public interface ISerialPort : IDisposable
{
    /// <summary>
    /// Send a command to the device.
    /// </summary>
    /// <param name="command">Command string - &lt;CR> is automatically added.</param>
    void WriteLine(string command);

    /// <summary>
    /// Wait for the next string from the device.
    /// </summary>
    /// <returns>The requested string.</returns>
    string ReadLine();
}
