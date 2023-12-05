using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Constraints;
using SerialPortProxy;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.Source;

namespace SourceApi.Tests.Actions.SerialPort;

[TestFixture]
public class FGSourceTests
{
    class LoggingSourceMock : SerialPortFGMock
    {
        public readonly List<string> Commands = new();

        public override void WriteLine(string command)
        {
            Commands.Add(command);

            base.WriteLine(command);
        }
    }

    private readonly NullLogger<SerialPortFGSource> _portLogger = new();

    private readonly NullLogger<ISerialPortConnection> _connectionLogger = new();

    private ISerialPortConnection _device;

    private SerialPortFGSource _source;

    private LoggingSourceMock _port;

    [SetUp]
    public void SetUp()
    {
        _port = new();
        _device = SerialPortConnection.FromPortInstance(_port, _connectionLogger);
        _source = new(_portLogger, _device, new CapabilitiesMap());

        _source.SetAmplifiers(Model.VoltageAmplifiers.VU220, Model.CurrentAmplifiers.VI220, Model.VoltageAuxiliaries.V210, Model.CurrentAuxiliaries.V200);
    }

    [TearDown]
    public void TearDown()
    {
        _device?.Dispose();
    }

    [TestCase(0.01, "IPAAR000.010000.00S000.020120.00T000.030240.00")]
    [TestCase(0.5, "IPAAR000.500000.00S001.000120.00T001.500240.00")]
    public async Task Can_Set_Valid_Loadpoint(double baseAngle, string current)
    {
        Assert.That(_source.GetCurrentLoadpoint(), Is.Null);

        var result = await _source.SetLoadpoint(new Model.Loadpoint
        {
            Frequency = new()
            {
                Mode = Model.FrequencyMode.SYNTHETIC,
                Value = 50
            },
            Phases = new() {
                new()  {
                    Current = new Model.ElectricalVectorQuantity { Rms=1 * baseAngle, Angle=0, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=220, Angle=0, On=true},
                },
                new() {
                    Current = new Model.ElectricalVectorQuantity { Rms=2 * baseAngle, Angle=120, On=true},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=221, Angle=120, On=false},
                },
                new() {
                    Current = new Model.ElectricalVectorQuantity { Rms=3 * baseAngle, Angle=240, On=false},
                    Voltage = new Model.ElectricalVectorQuantity { Rms=222, Angle=240, On=true},
                },
            },
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceResult.SUCCESS));

        Assert.That(_port.Commands, Is.EqualTo(new string[] {
            "FR50.00",
            "UPAER220.000000.00S221.000120.00T222.000240.00",
            current,
            "UIEAEPPAAAA"
        }));

        var loadpoint = _source.GetCurrentLoadpoint();

        Assert.That(loadpoint, Is.Not.Null);
        Assert.That(loadpoint.Frequency.Value, Is.EqualTo(50));
    }

}
