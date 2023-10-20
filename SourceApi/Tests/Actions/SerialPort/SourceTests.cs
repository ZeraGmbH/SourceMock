using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.SerialPort;
using WebSamDeviceApis.Actions.Source;

namespace WebSamDeviceApis.Tests.Actions.SerialPort;

[TestFixture]
public class SourceTests
{
    private readonly ILogger<SerialPortSource> _logger = new NullLogger<SerialPortSource>();

    private readonly SerialPortService _service = new(new SerialPortConfiguration
    {
        UseMockType = true,
        PortNameOrMockType = typeof(SerialPortMock).AssemblyQualifiedName!
    });

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
                    Current = new Model.ElectricalVectorQuantity { Rms=1, Angle=0, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=220, Angle=0, On=true},
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

        Assert.That(result, Is.EqualTo(SourceResult.SUCCESS));

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
