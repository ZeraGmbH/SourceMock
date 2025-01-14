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
    /// <param name="cancel">Can be provided to abort the operation.</param>
    void WriteLine(string command, CancellationToken? cancel = null);

    /// <summary>
    /// Wait for the next string from the device.
    /// </summary>
    /// <param name="cancel">Can be provided to abort the operation.</param>
    /// <returns>The requested string.</returns>
    string ReadLine(CancellationToken? cancel = null);

    /// <summary>
    /// Send raw data to the port.
    /// </summary>
    /// <param name="command">Bytes to send</param>
    /// <param name="cancel">Can be provided to abort the operation.</param>
    void RawWrite(byte[] command, CancellationToken? cancel = null);

    /// <summary>
    /// Read the next byte of data from the port.
    /// </summary>
    /// <param name="cancel">Can be provided to abort the operation.</param>
    public byte? RawRead(CancellationToken? cancel = null);
}
