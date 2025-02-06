using System.Text.RegularExpressions;

using SerialPortProxy;

namespace ZeraDevices.Mocks;

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

    private static readonly Regex ThreePs45Command = new(@"^3PS45;(.+)$");

    private static readonly Regex OlCommand = new(@"^OL(0|1){16}$");

    private static readonly Regex HpCommand = new(@"^HP(0|1){2}$");

    private static readonly Regex HeCommand = new(@"^HE(A|E){16}$");

    private static readonly Regex twoSCommand = new(@"^2S(1|0)(.+)$");

    private static readonly Regex twoXCommand = new(@"^2X(.+)$");

    private static readonly Regex ofCommand = new(@"^OF(0|1){8}(\+|-)(.+){3}\.(.+){3}$");

    private readonly Queue<QueueEntry> _replies = new();

    private DateTime _dosageStart = DateTime.MinValue;

    private bool _dosageActive = false;

    private bool _dosageMode = false;

    private double _energy = 0;

    /// <inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <inheritdoc/>
    public byte? RawRead(int? timeout = null) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void RawWrite(byte[] command) => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string ReadLine()
    {
        if (!_replies.TryDequeue(out var info))
        {
            Thread.Sleep(100);

            throw new TimeoutException("no reply in queue");
        }

        return info.Reply;
    }

    private int DosageProgress => Math.Min((int)((DateTime.Now - _dosageStart).TotalSeconds * 10), 100);

    /// <summary>
    /// Generate a random factor.
    /// </summary>
    /// <param name="number"></param>
    /// <returns>Number scaled wit a random factor between -1% and +1%.</returns>
    private static double MockNumber(double number) => number * (1.0 + Random.Shared.Next(-1000, +1000) / 100000.0);

    /// <inheritdoc/>
    public virtual void WriteLine(string command)
    {
        switch (command)
        {
            case "TS":
                _replies.Enqueue("TSFG301   V299");
                break;
            case "MI":
                _replies.Enqueue("MI4LW;3LW;4LBK;4LBE;3LBK;3LBA;3BKB;3LBE;3LWR;4LBF;1PHT;1PHR;1PHA;4LS;3LS;4LQ6;3LQ6;4Q6K;3Q6K;4LSG;3LSG;4LBG;3LBG;");
                break;
            case "BU":
                /* Range Voltage: 250V */
                _replies.Enqueue("BU250.000");
                break;
            case "AU":
                /* Voltage: 200V, 250V, 300V */
                _replies.Enqueue($"AUR{MockNumber(16000):00000}S{MockNumber(20000):00000}T{MockNumber(24000):00000}");
                break;
            case "BI":
                // Range Current: 5A */
                _replies.Enqueue("BI5.000");
                break;
            case "AI":
                /* Current: 2A, 1A, 3A */
                _replies.Enqueue($"AIR{MockNumber(8000):00000}S{MockNumber(4000):00000}T{MockNumber(12000):00000}");
                break;
            case "AW":
                /* Angle (V,A): (0, 5), (110, 130), (245, 231) */
                _replies.Enqueue($"AWR{MockNumber(0):000.0}{MockNumber(5):000.0}S{MockNumber(110):000.0}{MockNumber(130):000.0}T{MockNumber(245):000.0}{MockNumber(231):000.0}");
                break;
            case "MP":
                /* Active Power: 400W, 235W, 900W => 1535W */
                _replies.Enqueue($"MPR{MockNumber(400):0.0};S{MockNumber(235):0.0};T{MockNumber(900):0.0};");
                break;
            case "MQ":
                /* Reactive Power: 35var, 85var, -80var => 40var */
                _replies.Enqueue($"MQR{MockNumber(35):0.0};S{MockNumber(85):0.0};T{MockNumber(-80):0.0};");
                break;
            case "MS":
                /* Apparent power: 400VA, 250VA, 900VA => 1550VA */
                _replies.Enqueue($"MSR{MockNumber(400):0.0};S{MockNumber(250):0.0};T{MockNumber(900):0.0};");
                break;
            case "AF":
                /* Frequency: 50Hz */
                _replies.Enqueue($"AF{MockNumber(50.0):00.00}");
                break;
            case "3CM1":
                _dosageStart = DateTime.Now;
                _dosageActive = true;
                _replies.Enqueue("OK3CM1");
                break;
            case "3CM2":
                _dosageActive = false;
                _replies.Enqueue("OK3CM2");
                break;
            case "3CM3":
                _dosageMode = true;
                _replies.Enqueue("OK3CM3");
                break;
            case "3CM4":
                _dosageMode = false;
                _replies.Enqueue("OK3CM4");
                break;
            case "3SA1":
                _replies.Enqueue($"OK3SA1;{(_dosageActive && DosageProgress < 100 ? "2" : "1")}");
                break;
            case "3SA3":
                _replies.Enqueue($"OK3SA3;{(_dosageMode ? "2" : "1")}");
                break;
            case "3MA1":
                _replies.Enqueue($"OK3MA1;{_energy * (100 - DosageProgress) / 100}");
                break;
            case "3SA4":
                _replies.Enqueue($"OK3SA4;{_energy * DosageProgress / 100}");
                break;
            case "3PA45":
                _replies.Enqueue($"OK3PA45;{_energy}");
                break;
            case "SE0":
            case "SE1":
                _replies.Enqueue("OKSE");
                break;
            case "SM":
                _replies.Enqueue(DateTime.Now.Minute % 2 == 0 ? "SM4200000200200000080000" : "SM0000000000000000000000");
                break;
            /* Reference meter nominal frequency. */
            case "FI":
                _replies.Enqueue("FI 60000");
                break;
            case "RE0":
            case "RE1":
                _replies.Enqueue("OKRE");
                break;
            case "PL":
                _replies.Enqueue("OKPL");
                break;
            case "SF0":
            case "SF1":
                _replies.Enqueue("OKSF");
                break;
            case "HEAAAAAAAAAAAAAAAA":
                _replies.Enqueue("OKHE");
                break;
            case "DS0":
                _replies.Enqueue("OKDS");
                break;
            case "DS1":
                _replies.Enqueue("OKDS");
                break;
            default:
                {
                    Match? match;

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
                    /* Set the measuring mode. */
                    else if (MaCommand.IsMatch(command))
                        _replies.Enqueue("OKMA");
                    else if (OlCommand.IsMatch(command))
                        _replies.Enqueue("OKOL");
                    else if (HpCommand.IsMatch(command))
                        _replies.Enqueue("OKHP");
                    else if (HeCommand.IsMatch(command))
                        _replies.Enqueue("OKHE");
                    else if (twoSCommand.IsMatch(command))
                        _replies.Enqueue("2OK");
                    else if (twoXCommand.IsMatch(command))
                        _replies.Enqueue("2OK");
                    else if (ofCommand.IsMatch(command))
                        _replies.Enqueue("OKOF");
                    /* Set dosage energy. */
                    else if ((match = ThreePs45Command.Match(command)).Success)
                    {
                        _energy = double.Parse(match.Groups[1].Value);

                        _replies.Enqueue("OK3PS45");
                    }
                    break;
                }

        }
    }
}
