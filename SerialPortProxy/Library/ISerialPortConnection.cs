namespace SerialPortProxy;

/// <summary>
/// Interface for a single serial port connection..
/// </summary>
public interface ISerialPortConnection : IDisposable
{
    /// <summary>
    /// Add a command to be sent to the serial port to the queue.
    /// </summary>
    /// <param name="requests">The command to send to the device.</param>
    /// <exception cref="ArgumentNullException">Parameter must not be null.</exception>
    /// <returns>All lines sent from the device as a task.</returns>
    Task<string[]>[] Execute(params SerialPortRequest[] requests);
}
