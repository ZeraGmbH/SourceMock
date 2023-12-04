using System.Text.RegularExpressions;
using MeteringSystemApi.Actions.Device;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Models;
using SerialPortProxy;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Model;

namespace MeteringSystemApiTests;

[TestFixture]
public class MeteringSystemTests
{
    class PortMock : ISerialPort
    {
        private static readonly Regex ZpCommand = new(@"^ZP\d{10}$");

        private readonly Queue<string> _queue = new();

        public void Dispose()
        {
        }

        public string ReadLine()
        {
            if (_queue.TryDequeue(out var reply))
                return reply;

            throw new TimeoutException("queue is empty");
        }

        public void WriteLine(string command)
        {
            switch (command)
            {
                case "AAV":
                    _queue.Enqueue("FG399V703");
                    _queue.Enqueue("AAVACK");

                    break;
                case "TS":
                    _queue.Enqueue("TSFG301   V385");
                    break;
                default:
                    if (ZpCommand.IsMatch(command))
                        _queue.Enqueue("OKZP");
                    break;
            }
        }
    }

    private readonly NullLogger<SerialPortFGMeteringSystem> _meterLogger = new();

    private readonly NullLogger<SerialPortFGSource> _sourceLogger = new();

    private PortMock _port = null!;

    private ISerialPortConnection Device = null!;

    private IMeteringSystem Generator = null!;

    private ISerialPortFGSource Source = null!;

    private IServiceProvider Services = null!;

    [SetUp]
    public void Setup()
    {
        _port = new();

        Device = SerialPortConnection.FromPortInstance(_port, new NullLogger<ISerialPortConnection>());

        Source = new SerialPortFGSource(_sourceLogger, Device, new CapabilitiesMap());

        var services = new ServiceCollection();

        services.AddSingleton(Source);

        Services = services.BuildServiceProvider();

        Generator = new SerialPortFGMeteringSystem(Device, _meterLogger, Services);
    }

    [TearDown]
    public void Teardown()
    {
        _port?.Dispose();
        Device?.Dispose();
    }

    [Test]
    public async Task Can_Get_Capabilities_For_FG()
    {
        var caps = await Generator.GetCapabilities();

        Assert.That(caps, Is.Not.Null);
    }

    [Test]
    public async Task Can_Not_Get_Capabilities_For_MT()
    {
        var generator = new SerialPortMTMeteringSystem(Device,
            new NullLogger<SerialPortMTMeteringSystem>(),
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), Device, new CapabilitiesMap()));

        var caps = await generator.GetCapabilities();

        Assert.That(caps, Is.Null);
    }

    [Test]
    public async Task Can_Get_Firmware_Version_For_MT()
    {
        var generator = new SerialPortMTMeteringSystem(Device,
            new NullLogger<SerialPortMTMeteringSystem>(),
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), Device, new CapabilitiesMap()));

        var version = await generator.GetFirmwareVersion();

        Assert.Multiple(() =>
        {
            Assert.That(version.ModelName, Is.EqualTo("FG399"));
            Assert.That(version.Version, Is.EqualTo("703"));
        });
    }

    [Test]
    public async Task Can_Get_Firmware_Version_For_FG()
    {
        var version = await Generator.GetFirmwareVersion();

        Assert.Multiple(() =>
        {
            Assert.That(version.ModelName, Is.EqualTo("FG301"));
            Assert.That(version.Version, Is.EqualTo("V385"));
        });
    }

    [TestCase(VoltageAmplifiers.VU211x012, CurrentAmplifiers.VI202x0, ReferenceMeters.COM3003, "")]
    [TestCase(VoltageAmplifiers.VU301x1, CurrentAmplifiers.VI202x0, ReferenceMeters.COM3003, "voltage")]
    [TestCase(VoltageAmplifiers.VU211x012, CurrentAmplifiers.VI301x1, ReferenceMeters.COM3003, "current")]
    [TestCase(VoltageAmplifiers.VU211x012, CurrentAmplifiers.VI202x0, ReferenceMeters.RMM303x6, "referenceMeter")]
    public async Task Can_Set_Amplifiers(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters meter, string error)
    {
        if (string.IsNullOrEmpty(error))
            await Generator.SetAmplifiersAndReferenceMeter(voltage, current, meter);
        else
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() => Generator.SetAmplifiersAndReferenceMeter(voltage, current, meter));

            Assert.That(exception.Message, Is.EqualTo(error));
        }
    }
}
