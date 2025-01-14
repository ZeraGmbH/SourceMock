using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApiTests.Actions.SerialPort;

[TestFixture]
public class FGSourceTests
{
    class LoggingSourceMock : SerialPortFGMock
    {
        public readonly List<string> Commands = [];

        public override void WriteLine(string command, CancellationToken? cancel)
        {
            Commands.Add(command);

            base.WriteLine(command, cancel);
        }
    }

    private readonly NullLogger<SerialPortFGSource> _portLogger = new();

    private readonly NullLogger<ISerialPortConnection> _connectionLogger = new();

    private ISerialPortConnection _device = null!;

    private SerialPortFGSource _source = null!;

    private LoggingSourceMock _port = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _port = new();
        _device = SerialPortConnection.FromMockedPortInstance(_port, _connectionLogger);
        _source = new(_portLogger, _device, new CapabilitiesMap(), new SourceCapabilityValidator());

        await _source.SetAmplifiersAsync(new NoopInterfaceLogger(), VoltageAmplifiers.VU220, CurrentAmplifiers.VI220, VoltageAuxiliaries.V210, CurrentAuxiliaries.V200);
    }

    [TearDown]
    public void TearDown()
    {
        _device?.Dispose();
    }

    [TestCase(0.01, "IPAAR000.010000.00S000.020240.00T000.030120.00")]
    [TestCase(0.5, "IPAAR000.500000.00S001.000240.00T001.500120.00")]
    public async Task Can_Set_Valid_Loadpoint_Async(double baseAngle, string current)
    {
        Assert.That(await _source.GetCurrentLoadpointAsync(new NoopInterfaceLogger()), Is.Null);

        var result = await _source.SetLoadpointAsync(new NoopInterfaceLogger(), new()
        {
            Frequency = new()
            {
                Mode = FrequencyMode.SYNTHETIC,
                Value = new(50)
            },
            Phases = [
                new()  {
                    Current = new() { AcComponent = new () {Rms=new(1 * baseAngle), Angle=new(0)}, On=true},
                    Voltage = new() { AcComponent = new () {Rms=new(220), Angle=new(0)}, On=true},
                },
                new() {
                    Current = new() { AcComponent = new () {Rms=new(2 * baseAngle), Angle=new(120)}, On=true},
                    Voltage = new() { AcComponent = new () {Rms=new(221), Angle=new(120)}, On=false},
                },
                new() {
                    Current = new() { AcComponent = new () {Rms=new(3 * baseAngle), Angle=new(240)}, On=false},
                    Voltage = new() { AcComponent = new () {Rms=new(222), Angle=new(240)}, On=true},
                },
            ],
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));

        Assert.That(_port.Commands, Is.EqualTo(new string[] {
            "DS1",
            "FR50.00",
            "UPAER220.000000.00S221.000240.00T222.000120.00",
            current,
            "UIEAEPPAAAA"
        }));

        var loadpoint = await _source.GetCurrentLoadpointAsync(new NoopInterfaceLogger());

        Assert.That(loadpoint, Is.Not.Null);
        Assert.That((double)loadpoint!.Frequency.Value, Is.EqualTo(50));
    }

    [Test]
    public async Task Can_Set_IEC_Loadpoint_Async()
    {
        Assert.That(await _source.GetCurrentLoadpointAsync(new NoopInterfaceLogger()), Is.Null);

        var result = await _source.SetLoadpointAsync(new NoopInterfaceLogger(), new TargetLoadpoint
        {
            Frequency = new()
            {
                Mode = FrequencyMode.SYNTHETIC,
                Value = new(50)
            },
            Phases = [
                new()  {
                    Current = new() { AcComponent = new () { Rms=new(10), Angle=new(0)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(120), Angle=new(330)}, On=true},
                },
                new() {
                    Current = new() { AcComponent = new () { Rms=new(10), Angle=new(240)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(120), Angle=new(210)}, On=true},
                },
                new() {
                    Current = new() { AcComponent = new () { Rms=new(10), Angle=new(120)}, On=true},
                    Voltage = new() { AcComponent = new () { Rms=new(120), Angle=new(90)}, On=true},
                },
            ],
            VoltageNeutralConnected = true,
        });

        Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));

        Assert.That(_port.Commands, Is.EqualTo(new string[] {
            "DS1",
            "FR50.00",
            "UPAER120.000000.00S120.000120.00T120.000240.00",
            "IPAAR010.000330.00S010.000090.00T010.000210.00",
            "UIEEEPPPAAA"
        }));

        var loadpoint = await _source.GetCurrentLoadpointAsync(new NoopInterfaceLogger());

        Assert.That(loadpoint, Is.Not.Null);
        Assert.That((double)loadpoint!.Frequency.Value, Is.EqualTo(50));
    }
}
