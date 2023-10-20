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

    private ILogger<SerialPortSource> _logger;

    private SerialPortService _service;

    [SetUp]
    public void SetUp()
    {
        _logger = new NullLogger<SerialPortSource>();

        _service = new(new SerialPortConfiguration
        {
            UseMockType = true,
            PortNameOrMockType = typeof(PortMock).AssemblyQualifiedName!
        });
    }

    [TearDown]
    public void TearDown()
    {
        _service?.Dispose();
    }

    [Test]
    public void Can_Get_Capabilities()
    {
        var sut = new SerialPortSource(_logger, _service);

        var caps = sut.GetCapabilities();

        Assert.That(caps.FrequencyRanges[0].Min, Is.EqualTo(45));
    }

    [Test]
    public void Can_Set_Valid_Loadpoint()
    {
        var sut = new SerialPortSource(_logger, _service);

        Assert.That(sut.GetCurrentLoadpoint(), Is.Null);

        var result = sut.SetLoadpoint(new Model.Loadpoint
        {
            Frequency = new Model.Frequency { Mode = Model.FrequencyMode.SYNTHETIC, Value = 50 },
            Phases = new List<Model.PhaseLoadpoint>() {
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=0.01, Angle=0, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=220, Angle=0, On=true},
                },
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=0.02, Angle=120, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=221, Angle=120, On=false},
                },
                new Model.PhaseLoadpoint {
                    Current = new Model.ElectricalVectorQuantity { Rms=0.03, Angle=240, On=false},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=222, Angle=240, On=true},
                },
            },
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceResult.SUCCESS));

        Assert.That(PortMock.Commands, Is.EqualTo(new string[] {
            "SUPAAR220.000000.00S221.000120.00T222.000240.00",
            "SIPAAR000.010000.00S000.020120.00T000.030240.00",
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
    public void Can_Set_Invalid_Loadpoint(int voltage, int current, int angle, SourceResult expectedError)
    {
        var sut = new SerialPortSource(_logger, _service);

        Assert.That(sut.GetCurrentLoadpoint(), Is.Null);

        var result = sut.SetLoadpoint(new Model.Loadpoint
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
