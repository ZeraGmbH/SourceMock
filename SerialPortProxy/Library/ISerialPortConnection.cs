using System.Text.RegularExpressions;
using SharedLibrary.Models.Logging;

namespace SerialPortProxy;

/// <summary>
/// Interface for a single serial port connection..
/// </summary>
public interface ISerialPortConnectionExecutor
{
    /// <summary>
    /// Add a command to be sent to the serial port to the queue.
    /// </summary>
    /// <param name="requests">The command to send to the device.</param>
    /// <exception cref="ArgumentNullException">Parameter must not be null.</exception>
    /// <returns>All lines sent from the device as a task.</returns>
    Task<string[]>[] Execute(params SerialPortRequest[] requests);
}

/// <summary>
/// Interface for a single serial port connection..
/// </summary>
public interface ISerialPortConnection : IDisposable
{
    /// <summary>
    /// Create scoped helper to execute commands.
    /// </summary>
    /// <param name="type">Type of the websam device using this connection.</param>
    /// <param name="id">Optional unique identifier of the device - only if multiple devices of the same type are used.</param>
    /// <returns>A new executor instance.</returns>
    ISerialPortConnectionExecutor CreateExecutor(InterfaceLogSourceTypes type, string id = "");

    /// <summary>
    /// Registeres Out-Of-Band essage handling.
    /// </summary>
    /// <param name="pattern">Pattern to recognize.</param>
    /// <param name="handler">Handler to process the message.</param>
    void RegisterEvent(Regex pattern, Action<Match> handler);
}
