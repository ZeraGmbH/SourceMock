using System.Text.RegularExpressions;

using SerialPortProxy;

namespace WebSamDeviceApis.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public class SerialPortFGMock : ISerialPort
{
    private static readonly Regex UpCommand = new(@"^UP([EA])([EA])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    private static readonly Regex IpCommand = new(@"^IP([EA])([AM])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    private static readonly Regex FrCommand = new(@"^FR(\d{2}\.\d{2})$");

    private static readonly Regex UiCommand = new(@"^UI([AE])([AE])([AE])([AP])([AP])([AP])([AE])([AE])([AE])$");

    private static readonly Regex ZpCommand = new(@"^ZP\d{10}$");

    private readonly Queue<QueueEntry> _replies = new();

    /// <inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <inheritdoc/>
    public virtual string ReadLine()
    {
        if (!_replies.TryDequeue(out var info))
            throw new TimeoutException("no reply in queue");

        return info.Reply;
    }

    /// <inheritdoc/>
    public virtual void WriteLine(string command)
    {
        switch (command)
        {
            case "TS":
                _replies.Enqueue("TSFG399   V703");
                break;
            default:
                {
                    /* Set voltage. */
                    if (UpCommand.IsMatch(command))
                        _replies.Enqueue("OKUP");
                    /* Set current.*/
                    else if (IpCommand.IsMatch(command))
                        _replies.Enqueue("OKIP");
                    /* Set frequency. */
                    else if (FrCommand.IsMatch(command))
                        _replies.Enqueue("OKFR");
                    /* Activate phases. */
                    else if (UiCommand.IsMatch(command))
                        _replies.Enqueue("OKUI");
                    /* Configure amplifiers and reference meter. */
                    else if (ZpCommand.IsMatch(command))
                        _replies.Enqueue("OKZP");

                    break;
                }

        }
    }
}
