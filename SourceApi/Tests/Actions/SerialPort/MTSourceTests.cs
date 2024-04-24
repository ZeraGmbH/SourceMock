using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;
using SharedLibrary.Actions;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;

namespace SourceApi.Tests.Actions.SerialPort;

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

        private readonly List<string> _commands = new();

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
    public async Task Can_Get_Capabilities()
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        var caps = await sut.GetCapabilities(new NoopInterfaceLogger());

        Assert.That(caps.FrequencyRanges[0].Min, Is.EqualTo(45));
    }

    [TestCase(0.01, "SIPAAR000.010000.00S000.020240.00T000.030120.00")]
    [TestCase(0.5, "SIPAAR000.500000.00S001.000240.00T001.500120.00")]
    public async Task Can_Set_Valid_Loadpoint(double baseAngle, string current)
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        Assert.That(sut.GetCurrentLoadpoint(new NoopInterfaceLogger()), Is.Null);

        var result = await sut.SetLoadpoint(new NoopInterfaceLogger(), new Model.TargetLoadpoint
        {
            Frequency = new Model.Frequency { Mode = Model.FrequencyMode.SYNTHETIC, Value = 50 },
            Phases = new List<Model.TargetLoadpointPhase>() {
                new Model.TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=1 * baseAngle, Angle=0}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=220, Angle=0}, On=true},
                },
                new Model.TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=2 * baseAngle, Angle=120}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=221, Angle=120}, On=false},
                },
                new Model.TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=3 * baseAngle, Angle=240}, On=false},
                    Voltage = new() { AcComponent = new () { Rms=222, Angle=240}, On=true},
                },
            },
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));

        Assert.That(PortMock.Commands, Is.EqualTo(new string[] {
            "SFR50.00",
            "SUPAER220.000000.00S221.000240.00T222.000120.00",
            current,
            "SUIEAEPPAAAA"
        }));

        var loadpoint = sut.GetCurrentLoadpoint(new NoopInterfaceLogger());

        Assert.That(loadpoint, Is.Not.Null);
        Assert.That(loadpoint.Frequency.Value, Is.EqualTo(50));
    }

    [TestCase(600, 1, 0, SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID)]
    [TestCase(220, 1000, 0, SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID)]
    [TestCase(220, 1, 700, SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID)]
    public async Task Can_Set_Invalid_Loadpoint(int voltage, int current, int angle, SourceApiErrorCodes expectedError)
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        Assert.That(sut.GetCurrentLoadpoint(new NoopInterfaceLogger()), Is.Null);

        var result = await sut.SetLoadpoint(new NoopInterfaceLogger(), new Model.TargetLoadpoint
        {
            Frequency = new Model.Frequency { Mode = Model.FrequencyMode.SYNTHETIC, Value = 50 },
            Phases = new List<Model.TargetLoadpointPhase>() {
                new Model.TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=current, Angle=angle}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=voltage, Angle=angle}, On=true},
                },
                new Model.TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=1, Angle=120}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=220, Angle=120}, On=true},
                },
                new Model.TargetLoadpointPhase {
                    Current = new() { AcComponent = new () { Rms=1, Angle=240}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=220, Angle=240}, On=true},
                },
            },
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(expectedError));
        Assert.That(sut.GetCurrentLoadpoint(new NoopInterfaceLogger()), Is.Null);
    }
}
