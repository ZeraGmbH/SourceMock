
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;
using SourceApi.Model;
using ZERA.WebSam.Shared.Actions;

namespace SourceApiTests.Actions.SerialPort;

[TestFixture]
public class MTSourceTests
{
    class PortMock : SerialPortMTMock
    {
        protected override bool UseDelay => false;

        private static PortMock? _current;

        public PortMock()
        {
            _current = this;
        }

        public override void Dispose()
        {
            _current = null;

            base.Dispose();
        }

        private readonly List<string> _commands = [];

        public static string[] Commands => _current!._commands.ToArray();

        public override void WriteLine(string command)
        {
            _commands.Add(command);

            base.WriteLine(command);
        }
    }

    private readonly NullLogger<SerialPortMTSource> _portLogger = new();

    private readonly NullLogger<ISerialPortConnection> _connectionLogger = new();

    private ISerialPortConnection _device;

    private readonly PortMock _portMock = new();

    [SetUp]
    public void SetUp()
    {
        _device = SerialPortConnection.FromMock<PortMock>(_connectionLogger);
    }

    [TearDown]
    public void TearDown()
    {
        _device?.Dispose();
    }

    [Test]
    public async Task Can_Get_Capabilities_Async()
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        var caps = await sut.GetCapabilitiesAsync(new NoopInterfaceLogger());

        Assert.That((double)caps.FrequencyRanges[0].Min, Is.EqualTo(45));
    }

    [TestCase(0.01, "SIPAAR000.010000.00S000.020240.00T000.030120.00")]
    [TestCase(0.5, "SIPAAR000.500000.00S001.000240.00T001.500120.00")]
    public async Task Can_Set_Valid_Loadpoint_Async(double baseAngle, string current)
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        Assert.That(await sut.GetCurrentLoadpointAsync(new NoopInterfaceLogger()), Is.Null);

        var result = await sut.SetLoadpointAsync(new NoopInterfaceLogger(), new TargetLoadpoint
        {
            Frequency = new GeneratedFrequency { Mode = FrequencyMode.SYNTHETIC, Value = new(50) },
            Phases = [
                new TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=new(1 * baseAngle), Angle=new(0)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(220), Angle=new(0)}, On=true},
                },
                new TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=new(2 * baseAngle), Angle=new(120)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(221), Angle=new(120)}, On=false},
                },
                new TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=new(3 * baseAngle), Angle=new(240)}, On=false},
                    Voltage = new() { AcComponent = new () { Rms=new(222), Angle=new(240)}, On=true},
                },
            ],
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));

        Assert.That(PortMock.Commands, Is.EqualTo(new string[] {
            "SFR50.00",
            "SUPAER220.000000.00S221.000240.00T222.000120.00",
            current,
            "SUIEAEPPAAAA"
        }));

        var loadpoint = await sut.GetCurrentLoadpointAsync(new NoopInterfaceLogger());

        Assert.That(loadpoint, Is.Not.Null);
        Assert.That((double)loadpoint.Frequency.Value, Is.EqualTo(50));
    }

    [TestCase(600, 1, 0, SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID)]
    [TestCase(220, 1000, 0, SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID)]
    [TestCase(220, 1, 700, SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID)]
    public async Task Can_Set_Invalid_Loadpoint_Async(int voltage, int current, int angle, SourceApiErrorCodes expectedError)
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        Assert.That(await sut.GetCurrentLoadpointAsync(new NoopInterfaceLogger()), Is.Null);

        var result = await sut.SetLoadpointAsync(new NoopInterfaceLogger(), new TargetLoadpoint
        {
            Frequency = new GeneratedFrequency { Mode = FrequencyMode.SYNTHETIC, Value = new(50) },
            Phases = [
                new TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=new(current), Angle=new(angle)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(voltage), Angle=new(angle)}, On=true},
                },
                new TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=new(1), Angle=new(120)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(220), Angle=new(120)}, On=true},
                },
                new TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=new(1), Angle=new(240)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(220), Angle=new(240)}, On=true},
                },
            ],
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(expectedError));
        Assert.That(await sut.GetCurrentLoadpointAsync(new NoopInterfaceLogger()), Is.Null);
    }

    [TestCase("AWR1")]
    [TestCase("AWR2")]
    [TestCase("AWR3")]
    [TestCase("AWR4")]
    [TestCase("AWR5")]
    public void Can_Mock_Awr_Command(string command)
    {
        _portMock.WriteLine(command);

        var actualReply = _portMock.ReadLine();

        Assert.That(actualReply, Is.EqualTo("AWRACK"));
    }

    [TestCase("AWR6")]
    [TestCase("AWR7")]
    [TestCase("AWR11")]
    [TestCase("AWR")]
    [TestCase("AWRR")]
    public void Not_Process_Wrong_Awr_Command(string command)
    {
        _portMock.WriteLine(command);

        Assert.Throws<TimeoutException>(() => _portMock.ReadLine());
    }

    [TestCase("AAMMAA")]
    [TestCase("AAMAAA")]
    [TestCase("AAMAMA")]
    [TestCase("AAMMMM")]
    [TestCase("AAMMMA")]
    public void Can_Mock_Aam_Command(string command)
    {
        _portMock.WriteLine(command);

        var actualReply = _portMock.ReadLine();

        Assert.That(actualReply, Is.EqualTo("AAMACK"));
    }

    [TestCase("AAMMMAA")]
    [TestCase("AAMCBG")]
    [TestCase("AAM")]
    [TestCase("AAMMM")]
    [TestCase("AAMM")]
    public void Not_Process_Wrong_Aam_Command(string command)
    {
        _portMock.WriteLine(command);

        Assert.Throws<TimeoutException>(() => _portMock.ReadLine());
    }

    [TestCase("AVR250")]
    [TestCase("AVR420")]
    public void Can_Mock_Avr_Command(string command)
    {
        _portMock.WriteLine("AAMMMM");

        Assert.That(_portMock.ReadLine(), Is.EqualTo("AAMACK"));

        _portMock.WriteLine(command);

        var actualReply = _portMock.ReadLine();

        Assert.That(actualReply, Is.EqualTo("AVRACK"));
    }

    [TestCase("AVR210")]
    [TestCase("AVRX60")]
    [TestCase("AVR111111111111111111111")] // max 20 
    [TestCase("AVR11111111111111111.111")]
    public void Not_Process_Wrong_Avr_Command(string command)
    {
        _portMock.WriteLine("AAMMMM");

        Assert.That(_portMock.ReadLine(), Is.EqualTo("AAMACK"));

        _portMock.WriteLine(command);

        Assert.That(_portMock.ReadLine(), Is.EqualTo("AVRNAK"));
    }

    [TestCase("ACR50")]
    [TestCase("ACR1")]
    [TestCase("ACR0.2")]
    public void Can_Mock_Acr_Command(string command)
    {
        _portMock.WriteLine("AAMMMM");

        Assert.That(_portMock.ReadLine(), Is.EqualTo("AAMACK"));

        _portMock.WriteLine(command);

        var actualReply = _portMock.ReadLine();

        Assert.That(actualReply, Is.EqualTo("ACRACK"));
    }

    [TestCase("ACR50B")]
    [TestCase("ACR51")]
    [TestCase("ACR50..01")]
    [TestCase("ACR50.1X")]
    [TestCase("ACR.1")]
    public void Not_Process_Wrong_Acr_Command(string command)
    {
        _portMock.WriteLine("AAMMMM");

        Assert.That(_portMock.ReadLine(), Is.EqualTo("AAMACK"));

        _portMock.WriteLine(command);

        Assert.That(_portMock.ReadLine(), Is.EqualTo("ACRNAK"));
    }
}
