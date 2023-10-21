using System.Text.RegularExpressions;

using SerialPortProxy;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// Simulate a very general moving test.
/// </summary>
public class SerialPortMock : ISerialPort
{
    private static readonly Regex _supCommand = new(@"^SUP([EA])([EA])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    private static readonly Regex _sipCommand = new(@"^SIP([EA])([AM])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    private static readonly Regex _sfrCommand = new(@"^SFR(\d{2}\.\d{2})$");

    private static readonly Regex _suiCommand = new(@"^SUI([AE])([AE])([AE])([AP])([AP])([AP])([AE])([AE])([AE])$");

    /// <summary>
    /// Outgoing messages.
    /// </summary>
    private readonly Queue<string> _replies = new();

    /// <inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Report the next outstanding reply string.
    /// </summary>
    /// <returns>Next string.</returns>
    public virtual string ReadLine()
    {
        if (_replies.TryDequeue(out var reply))
            return reply;

        throw new TimeoutException("no reply in quuue");
    }

    /// <summary>
    /// Simulate a command.
    /// </summary>
    /// <param name="command">Command to simulate.</param>
    public virtual void WriteLine(string command)
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
                        _replies.Enqueue("SOKUP");
                    else if (_sipCommand.IsMatch(command))
                        _replies.Enqueue("SOKIP");
                    else if (_sfrCommand.IsMatch(command))
                        _replies.Enqueue("SOKFR");
                    else if (_suiCommand.IsMatch(command))
                        _replies.Enqueue("SOKUI");

                    break;
                }

        }
    }
}
