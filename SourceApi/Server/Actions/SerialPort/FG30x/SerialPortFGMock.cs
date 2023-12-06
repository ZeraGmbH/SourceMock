using System.Text.RegularExpressions;

using SerialPortProxy;

namespace SourceApi.Actions.SerialPort.FG30x;

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

    private static readonly Regex MaCommand = new(@"^MA(.+)$");

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
                _replies.Enqueue("TSFG301   V385");
                break;
            case "MI":
                _replies.Enqueue("MI4LW;3LW;4LBK;4LBE;3LBK;3LBA;3BKB;3LBE;3LWR;4LBF;1PHT;1PHR;1PHA;4LS;3LS;4LQ6;3LQ6;4Q6K;3Q6K;4LSG;3LSG;4LBG;3LBG;");
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
                    /** Set the measuring mode. */
                    else if (MaCommand.IsMatch(command))
                        _replies.Enqueue("OKMA");

                    break;
                }

        }
    }
}
