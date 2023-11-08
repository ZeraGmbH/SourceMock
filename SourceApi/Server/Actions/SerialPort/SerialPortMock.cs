using System.Globalization;
using System.Text.RegularExpressions;

using SerialPortProxy;

namespace WebSamDeviceApis.Actions.SerialPort;

static class AMEReplyEmulator
{
    private static readonly string[] MockData = {
        "0;1.9996199E+01",
        "1;1.9995480E+01",
        "2;2.0000074E+01",
        "3;9.9997595E-02",
        "4;9.9997699E-02",
        "5;1.0000440E-01",
        "6;0.0000000E+00",
        "7;1.1999798E+02",
        "8;2.3999924E+02",
        "9;7.7056885E-04",
        "10;1.1999476E+02",
        "11;2.3999344E+02",
        "12;9.9965084E-01",
        "13;9.9996138E-01",
        "14;9.9992412E-01",
        "15;1.9988739E+00",
        "16;1.9994249E+00",
        "17;1.9999436E+00",
        "18;2.4177787E-05",
        "19;-1.1213896E-04",
        "20;-2.0453637E-04",
        "21;1.9995722E+00",
        "22;1.9995022E+00",
        "23;2.0000954E+00",
        "24;5.9982424E+00",
        "25;-2.9249751E-04",
        "26;5.9991698E+00",
        "27;123",
        "28;50.01",
        "29;1.5582092E-01",
        "30;1.3959724E-01",
        "31;1.3618226E-01",
        "32;2.0045177E-04",
        "33;1.1796872E-04",
        "34;9.6725911E-05",
        "35;1",
        "51;1.3298696E+00",
        "52;8.5928971E-01",
        "53;1.2193114E+00",
        "54;2.2211032E+00",
        "55;2.8615168E-01",
        "56;2.7889857E-01",
        "57;9.9984539E-01",
    };

    public static IEnumerable<string> GetReplies()
    {
        foreach (var reply in MockData)
        {
            /* Get the fallback value. */
            var parts = reply.Split(";");
            var index = int.Parse(parts[0], CultureInfo.InvariantCulture);
            var num = double.Parse(parts[1], CultureInfo.InvariantCulture);

            /* If a regular number just apply a random factor between 0.99 and 1.01 to the value. */
            if (index != 27 && index != 35)
                num *= Random.Shared.Next(99000, 101000) / 100000.0;

            yield return $"{parts[0]};{num.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}

/// <summary>
/// Represents a single reply from the device.
/// </summary>
class QueueEntry
{
    /// <summary>
    /// The reply text.
    /// </summary>
    public readonly string Reply;

    /// <summary>
    /// Optional delay to simulate real device behaviour.
    /// </summary>
    public readonly int Delay;

    /// <summary>
    /// Create a new reply entry.
    /// </summary>
    /// <param name="reply">Message to reply.</param>
    /// <param name="delay">Number of milliseconds to wait before the message is sent.</param>
    public QueueEntry(string reply, int delay)
    {
        Delay = delay;
        Reply = reply;
    }

    /// <summary>
    /// Povide auto conversion in the regular case with no delay.
    /// </summary>
    /// <param name="reply">Message to use as a reply, delay is assumed to be zero.</param>
    public static implicit operator QueueEntry(string reply) { return new QueueEntry(reply, 0); }
}

/// <summary>
/// Simulate a very general moving test.
/// </summary>
public class SerialPortMock : ISerialPort
{
    private static readonly Regex SupCommand = new(@"^SUP([EA])([EA])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    private static readonly Regex SipCommand = new(@"^SIP([EA])([AM])R(\d{3}\.\d{3})(\d{3}\.\d{2})S(\d{3}\.\d{3})(\d{3}\.\d{2})T(\d{3}\.\d{3})(\d{3}\.\d{2})$");

    private static readonly Regex SfrCommand = new(@"^SFR(\d{2}\.\d{2})$");

    private static readonly Regex SuiCommand = new(@"^SUI([AE])([AE])([AE])([AP])([AP])([AP])([AE])([AE])([AE])$");

    private static readonly Regex AtiCommand = new(@"^ATI(0[1-9]|[1-9]\d)$");

    private static readonly Regex AmtCommand = new(@"^AMT(.{1,4})$");

    private static readonly Regex S3ps46Command = new(@"^S3PS46;(.+)$");

    private string _measurementMode = "2WA";

    /// <summary>
    /// Outgoing messages.
    /// </summary>
    private readonly Queue<QueueEntry> _replies = new();

    /// <inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Reset in a unit test environment.
    /// </summary>
    protected virtual bool UseDelay => true;

    /// <summary>
    /// Report the next outstanding reply string.
    /// </summary>
    /// <returns>Next string.</returns>
    public virtual string ReadLine()
    {
        if (!_replies.TryDequeue(out var info))
            throw new TimeoutException("no reply in quuue");

        /* Simulate delay. */
        if (UseDelay && info.Delay > 0)
            Thread.Sleep(info.Delay);

        return info.Reply;
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
            /* Read the current values. */
            case "AME":
                {
                    foreach (var reply in AMEReplyEmulator.GetReplies())
                        _replies.Enqueue(reply);

                    _replies.Enqueue(new QueueEntry("AMEACK", 1000));
                }
                break;
            /* Read the list of supported measurement modes. */
            case "AML":
                {
                    _replies.Enqueue("01;4WA;4LW");
                    _replies.Enqueue("02;4WR;4LBE");
                    _replies.Enqueue("03;4WRC;4LBK");
                    _replies.Enqueue("04;4WAP;4LS");
                    _replies.Enqueue("05;4WAb;4LWb");
                    _replies.Enqueue("06;4WRb;4LBb");
                    _replies.Enqueue("07;4WAPb;4LSb");
                    _replies.Enqueue("08;3WA;3LW");
                    _replies.Enqueue("09;3WR;3LBE");
                    _replies.Enqueue("10;3WRCA;3LBKA");
                    _replies.Enqueue("11;3WRCB;3LBKB");
                    _replies.Enqueue("12;3WAP;3LS");
                    _replies.Enqueue("13;2WA;2LW");
                    _replies.Enqueue("14;2WR;2LB");
                    _replies.Enqueue("15;2WAP;2LS");
                    _replies.Enqueue("16;MQRef;MQRef");
                    _replies.Enqueue("17;MQBase;MQBase");
                    _replies.Enqueue("AMLACK");

                    break;
                }
            /* Get the current status, esp. the measurement mode */
            case "AST":
                {
                    _replies.Enqueue("UB=420");
                    _replies.Enqueue("IB=100");
                    _replies.Enqueue($"M={_measurementMode}");
                    _replies.Enqueue("PLL=U1");
                    _replies.Enqueue("UEB=--");
                    _replies.Enqueue("NIB=0");
                    _replies.Enqueue("RDY=11-");
                    _replies.Enqueue("ASTACK");

                    break;
                }
            /* Start dosage. */
            case "S3CM1":
                {
                    _replies.Enqueue("SOK3CM1");

                    break;
                }
            /* Abort dosage. */
            case "S3CM2":
                {
                    _replies.Enqueue("SOK3CM2");

                    break;
                }
            /* Set DOS mode. */
            case "S3CM3":
                {
                    _replies.Enqueue("SOK3CM3");

                    break;
                }
            /* Reset DOS mode. */
            case "S3CM4":
                {
                    _replies.Enqueue("SOK3CM4");

                    break;
                }
            /* Read dosage status. */
            case "S3SA1":
                {
                    _replies.Enqueue("SOK3SA1;1");

                    break;
                }
            /* Read total number of impulses. */
            case "S3SA5":
                {
                    _replies.Enqueue("SOK3SA5;1127813");

                    break;
                }
            /* Read remaining pulses. */
            case "S3MA4":
                {
                    _replies.Enqueue("SOK3MA4;130129");

                    break;
                }
            /* Read pulses processed. */
            case "S3MA5":
                {
                    _replies.Enqueue("SOK3MA5;26671");

                    break;
                }
            default:
                {
                    /* Set voltage. */
                    if (SupCommand.IsMatch(command))
                        _replies.Enqueue("SOKUP");
                    /* Set current.*/
                    else if (SipCommand.IsMatch(command))
                        _replies.Enqueue("SOKIP");
                    /* Set frequency. */
                    else if (SfrCommand.IsMatch(command))
                        _replies.Enqueue("SOKFR");
                    /* Activate phases. */
                    else if (SuiCommand.IsMatch(command))
                        _replies.Enqueue(new QueueEntry("SOKUI", 5000));
                    /* Set integration time. */
                    else if (AtiCommand.IsMatch(command))
                        _replies.Enqueue("ATIACK");
                    /* Set dosage energy. */
                    else if (S3ps46Command.IsMatch(command))
                        _replies.Enqueue("SOK3PS46");
                    /* Set measurement mode. */
                    else
                    {
                        /* On AMT remember the new measurement mode. */
                        var match = AmtCommand.Match(command);

                        if (match.Success)
                        {
                            _measurementMode = match.Groups[1].Value;

                            _replies.Enqueue("AMTACK");
                        }
                    }

                    break;
                }

        }
    }
}
