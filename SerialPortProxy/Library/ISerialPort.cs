using ZERA.WebSam.Shared.Models;

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

    /// <summary>
    /// Send raw data to the port.
    /// </summary>
    /// <param name="command">Bytes to send</param>
    void RawWrite(byte[] command);

    /// <summary>
    /// Read the next byte of data from the port.
    /// </summary>
    /// <param name="timeout">Optional timeout in milliseconds.</param>
    public byte? RawRead(int? timeout = null);
}

/// <summary>
/// Helper methods for a serial port.
/// </summary>
public static class ISerialPortExtensions
{
    /// <summary>
    /// Read a single byte with timeout and the possiblity to cancel.
    /// </summary>
    /// <param name="port">Serial port to use.</param>
    /// <param name="timeout">Maximum time to read the byte in milliseconds.</param>
    /// <param name="cancel">Optional cancel manager.</param>
    /// <returns>Next byte from the port or null if port is closed.</returns>
    public static byte? RawRead(this ISerialPort port, int timeout, ICancellationService? cancel)
    {
        if (cancel == null) return port.RawRead();

        for (var end = DateTime.UtcNow.AddMilliseconds(timeout); ;)
            try
            {
                /* May cancel each 250ms. */
                return port.RawRead(250);
            }
            catch (TimeoutException)
            {
                /* The outer timeout has been reached. */
                if (DateTime.UtcNow >= end) throw;

                cancel.ThrowIfCancellationRequested();
            }
    }
}