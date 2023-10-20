using System.Text.RegularExpressions;

using SerialPortProxy;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// Simulate a very general moving test.
/// </summary>
public class SerialPortMock : ISerialPort
{
    private static readonly Regex _supCommand = new(@"^SUP([EA])([EA])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    /// <summary>
    /// Outgoing messages.
    /// </summary>
    private readonly Queue<string> _replies = new();

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <summary>
    /// Report the next outstanding reply string.
    /// </summary>
    /// <returns>Next string.</returns>
    public string ReadLine() => this._replies.Dequeue();

    /// <summary>
    /// Simulate a command.
    /// </summary>
    /// <param name="command">Command to simulate.</param>
    public void WriteLine(string command)
    {
        switch (command)
        {
            /* Read the firmware version. */
            case "AAV":
                {
                    _replies.Enqueue("MT793V05.52");
                    _replies.Enqueue("AAVACK");

                    break;
                }
            default:
                {
                    if (_supCommand.IsMatch(command))
                    {
                        _replies.Enqueue("SOKUP");
                    }

                    break;
                }

        }
    }
}
