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
            case "AME":
                {
                    _replies.Enqueue("0;1.9996199E+01");
                    _replies.Enqueue("1;1.9995480E+01");
                    _replies.Enqueue("2;2.0000074E+01");
                    _replies.Enqueue("3;9.9997595E-02");
                    _replies.Enqueue("4;9.9997699E-02");
                    _replies.Enqueue("5;1.0000440E-01");
                    _replies.Enqueue("6;0.0000000E+00");
                    _replies.Enqueue("7;1.1999798E+02");
                    _replies.Enqueue("8;2.3999924E+02");
                    _replies.Enqueue("9;7.7056885E-04");
                    _replies.Enqueue("10;1.1999476E+02");
                    _replies.Enqueue("11;2.3999344E+02");
                    _replies.Enqueue("12;9.9965084E-01");
                    _replies.Enqueue("13;9.9996138E-01");
                    _replies.Enqueue("14;9.9992412E-01");
                    _replies.Enqueue("15;1.9988739E+00");
                    _replies.Enqueue("16;1.9994249E+00");
                    _replies.Enqueue("17;1.9999436E+00");
                    _replies.Enqueue("18;2.4177787E-05");
                    _replies.Enqueue("19;-1.1213896E-04");
                    _replies.Enqueue("20;-2.0453637E-04");
                    _replies.Enqueue("21;1.9995722E+00");
                    _replies.Enqueue("22;1.9995022E+00");
                    _replies.Enqueue("23;2.0000954E+00");
                    _replies.Enqueue("24;5.9982424E+00");
                    _replies.Enqueue("25;-2.9249751E-04");
                    _replies.Enqueue("26;5.9991698E+00");
                    _replies.Enqueue("27;123");
                    _replies.Enqueue("28;50.01");
                    _replies.Enqueue("29;1.5582092E-01");
                    _replies.Enqueue("30;1.3959724E-01");
                    _replies.Enqueue("31;1.3618226E-01");
                    _replies.Enqueue("32;2.0045177E-04");
                    _replies.Enqueue("33;1.1796872E-04");
                    _replies.Enqueue("34;9.6725911E-05");
                    _replies.Enqueue("35;1");
                    _replies.Enqueue("51;1.3298696E+00");
                    _replies.Enqueue("52;8.5928971E-01");
                    _replies.Enqueue("53;1.2193114E+00");
                    _replies.Enqueue("54;2.2211032E+00");
                    _replies.Enqueue("55;2.8615168E-01");
                    _replies.Enqueue("56;2.7889857E-01");
                    _replies.Enqueue("57;9.9984539E-01");
                    _replies.Enqueue("AMEACK");
                }
                break;
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
