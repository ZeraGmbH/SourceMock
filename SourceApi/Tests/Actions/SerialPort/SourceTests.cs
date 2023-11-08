using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.SerialPort;
using WebSamDeviceApis.Actions.Source;

namespace WebSamDeviceApis.Tests.Actions.SerialPort;

[TestFixture]
public class SourceTests
{
    class PortMock : SerialPortMock
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

    private readonly NullLogger<SerialPortSource> _portLogger = new();

    private readonly NullLogger<SerialPortConnection> _connectionLogger = new();

    private SerialPortConnection _device;

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
        var sut = new SerialPortSource(_portLogger, _device);

        var caps = await sut.GetCapabilities();

        Assert.That(caps.FrequencyRanges[0].Min, Is.EqualTo(45));
    }

    [TestCase(0.01, "SIPAAR000.010000.00S000.020120.00T000.030240.00")]
    [TestCase(0.5, "SIPAAR000.500000.00S001.000120.00T001.500240.00")]
    public async Task Can_Set_Valid_Loadpoint(double baseAngle, string current)
    {
        var sut = new SerialPortSource(_portLogger, _device);

        Assert.That(sut.GetCurrentLoadpoint(), Is.Null);

        var result = await sut.SetLoadpoint(new Model.Loadpoint
        {
            Frequency = new Model.Frequency { Mode = Model.FrequencyMode.SYNTHETIC, Value = 50 },
            Phases = new List<Model.PhaseLoadpoint>() {
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=1 * baseAngle, Angle=0, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=220, Angle=0, On=true},
                },
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=2 * baseAngle, Angle=120, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=221, Angle=120, On=false},
                },
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=3 * baseAngle, Angle=240, On=false},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=222, Angle=240, On=true},
                },
            },
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceResult.SUCCESS));

        Assert.That(PortMock.Commands, Is.EqualTo(new string[] {
            "SUPAER220.000000.00S221.000120.00T222.000240.00",
            current,
            "SFR50.00",
            "SUIEAEPPAAAA"
        }));

        var loadpoint = sut.GetCurrentLoadpoint();

        Assert.That(loadpoint, Is.Not.Null);
        Assert.That(loadpoint.Frequency.Value, Is.EqualTo(50));
    }

    [TestCase(600, 1, 0, SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID)]
    [TestCase(220, 1000, 0, SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID)]
    [TestCase(220, 1, 700, SourceResult.LOADPOINT_ANGLE_INVALID)]
    public async Task Can_Set_Invalid_Loadpoint(int voltage, int current, int angle, SourceResult expectedError)
    {
        var sut = new SerialPortSource(_portLogger, _device);

        Assert.That(sut.GetCurrentLoadpoint(), Is.Null);

        var result = await sut.SetLoadpoint(new Model.Loadpoint
        {
            Frequency = new Model.Frequency { Mode = Model.FrequencyMode.SYNTHETIC, Value = 50 },
            Phases = new List<Model.PhaseLoadpoint>() {
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=current, Angle=angle, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=voltage, Angle=angle, On=true},
                },
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=1, Angle=120, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=220, Angle=120, On=true},
                },
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=1, Angle=240, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=220, Angle=240, On=true},
                },
            },
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(expectedError));
        Assert.That(sut.GetCurrentLoadpoint(), Is.Null);
    }
}
