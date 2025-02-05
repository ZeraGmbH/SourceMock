using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;
using ZeraDevices.Source.MT768;
using ZeraDevices.Source;
using ZERA.WebSam.Shared.Provider;
using Moq;

namespace ZeraDeviceTests;

[TestFixture]
public class MTDosageTests
{
    private readonly ISourceCapabilityValidator Validator = new Mock<ISourceCapabilityValidator>().Object;

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

    private readonly NullLogger<ISerialPortConnection> _connectionLogger = new();

    private readonly NullLogger<SerialPortMTSource> _portLogger = new();

    private readonly IInterfaceLogger _logger = new NoopInterfaceLogger();

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
    public async Task Run_Energy_Measurement_Async()
    {
        var sut = new SerialPortMTSource(_portLogger, _device, new CapabilitiesMap(), Validator);

        await sut.SetLoadpointAsync(_logger, new()
        {
            Frequency = { Mode = FrequencyMode.SYNTHETIC, Value = new(50) },
            Phases = [
                new () {
                    Current = { AcComponent = new () { Rms=new(1), Angle=new(0)}, On=true},
                    Voltage = { AcComponent = new () { Rms=new(220), Angle=new(0)}, On=true},
                },
                new () {
                    Current = { AcComponent = new () { Rms=new(2), Angle=new(120)}, On=true},
                    Voltage = { AcComponent = new () { Rms=new(221), Angle=new(120)}, On=false},
                },
                new () {
                    Current = { AcComponent = new () { Rms=new(3), Angle=new(240)}, On=false},
                    Voltage = { AcComponent = new () { Rms=new(222), Angle=new(240)}, On=true},
                },
            ],
            VoltageNeutralConnected = true,
        });

        Assert.That(PortMock.Commands.Where(c => c.StartsWith("AE")).ToArray(), Is.EqualTo(Array.Empty<string>()));

        await sut.StartEnergyAsync(_logger);

        Assert.That(PortMock.Commands.Where(c => c.StartsWith("AE")).ToArray(), Is.EqualTo(new string[] { "AET1" }));

        var energy = await sut.GetEnergyAsync(_logger);

        Assert.That((double)energy, Is.EqualTo(3.141592d));

        Assert.That(PortMock.Commands.Where(c => c.StartsWith("AE")).ToArray(), Is.EqualTo(new string[] { "AET1", "AEV" }));

        await sut.StopEnergyAsync(_logger);

        Assert.That(PortMock.Commands.Where(c => c.StartsWith("AE")).ToArray(), Is.EqualTo(new string[] { "AET1", "AEV", "AET0" }));
    }
}