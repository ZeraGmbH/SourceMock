using System.Text.RegularExpressions;
using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions.Device;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace MeterTestSystemApiTests;

[TestFixture]
public class MeterTestSystemTests
{
    class PortMock : ISerialPort
    {
        public readonly List<string> Commands = new();

        private static readonly Regex ZpCommand = new(@"^ZP\d{10}$");

        private readonly Queue<string> _queue = new();

        public void Dispose()
        {
        }

        public string ReadLine()
        {
            if (_queue.TryDequeue(out var reply))
                return reply;

            Thread.Sleep(100);

            throw new TimeoutException("queue is empty");
        }

        public void WriteLine(string command)
        {
            Commands.Add(command);

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

    private readonly NullLogger<SerialPortFGMeterTestSystem> _meterLogger = new();

    private readonly NullLogger<SerialPortFGSource> _sourceLogger = new();

    private PortMock _port = null!;

    private ISerialPortConnection Device = null!;

    private IMeterTestSystem Generator = null!;

    private ISerialPortFGSource Source = null!;

    private ServiceProvider Services = null!;

    [SetUp]
    public void Setup()
    {
        _port = new();

        Device = SerialPortConnection.FromPortInstance(_port, new NullLogger<ISerialPortConnection>());

        Source = new SerialPortFGSource(_sourceLogger, Device, new CapabilitiesMap(), new SourceCapabilityValidator());

        var services = new ServiceCollection();

        services.AddSingleton(Source);
        services.AddSingleton<ISerialPortFGRefMeter>(new SerialPortFGRefMeter(Device, new NullLogger<SerialPortFGRefMeter>()));
        services.AddSingleton<ISerialPortFGErrorCalculator>(new SerialPortFGErrorCalculator(Device, new NullLogger<SerialPortFGErrorCalculator>()));

        Services = services.BuildServiceProvider();

        Generator = new SerialPortFGMeterTestSystem(Device, _meterLogger, Services);
    }

    [TearDown]
    public void Teardown()
    {
        _port?.Dispose();
        Device?.Dispose();
        Services?.Dispose();
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
        var generator = new SerialPortMTMeterTestSystem(Device,
            new SerialPortMTRefMeter(Device, new NullLogger<SerialPortMTRefMeter>()),
            new SerialPortMTErrorCalculator(Device, new NullLogger<SerialPortMTErrorCalculator>()),
            new NullLogger<SerialPortMTMeterTestSystem>(),
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), Device, new CapabilitiesMap(), new SourceCapabilityValidator()));

        var caps = await generator.GetCapabilities();

        Assert.That(caps, Is.Null);
    }

    [Test]
    public async Task Can_Get_Firmware_Version_For_MT()
    {
        var generator = new SerialPortMTMeterTestSystem(Device,
            new SerialPortMTRefMeter(Device, new NullLogger<SerialPortMTRefMeter>()),
            new SerialPortMTErrorCalculator(Device, new NullLogger<SerialPortMTErrorCalculator>()),
            new NullLogger<SerialPortMTMeterTestSystem>(),
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), Device, new CapabilitiesMap(), new SourceCapabilityValidator()));

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

    [TestCase(VoltageAmplifiers.VU211x0, CurrentAmplifiers.VI202x0, ReferenceMeters.COM3003, "ZP3036012150")]
    [TestCase(VoltageAmplifiers.LABSMP21200, CurrentAmplifiers.VI202x0, ReferenceMeters.COM3003, "voltage")]
    [TestCase(VoltageAmplifiers.VU211x1, CurrentAmplifiers.LABSMP715, ReferenceMeters.COM3003, "current")]
    [TestCase(VoltageAmplifiers.VU211x2, CurrentAmplifiers.VI202x0, ReferenceMeters.RMM303x6, "referenceMeter")]
    public async Task Can_Set_Amplifiers(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters meter, string errorOrResponse)
    {
        if (errorOrResponse.StartsWith("ZP"))
        {
            await Generator.SetAmplifiersAndReferenceMeter(new()
            {
                CurrentAmplifier = current,
                CurrentAuxiliary = CurrentAuxiliaries.V200,
                ReferenceMeter = meter,
                VoltageAmplifier = voltage,
                VoltageAuxiliary = VoltageAuxiliaries.V210,
            });

            Assert.That(_port.Commands.Count, Is.EqualTo(1));
            Assert.That(_port.Commands[0], Is.EqualTo(errorOrResponse));
        }
        else
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() => Generator.SetAmplifiersAndReferenceMeter(new()
            {
                CurrentAmplifier = current,
                CurrentAuxiliary = CurrentAuxiliaries.V200,
                ReferenceMeter = meter,
                VoltageAmplifier = voltage,
                VoltageAuxiliary = VoltageAuxiliaries.V210,
            }));

            Assert.That(exception.Message, Is.EqualTo(errorOrResponse));
        }
    }

    [TestCase(VoltageAuxiliaries.VU211x0, CurrentAuxiliaries.VI202x0, ReferenceMeters.COM3003, "ZP0323303650")]
    [TestCase(VoltageAuxiliaries.SVG150x00, CurrentAuxiliaries.VI202x0, ReferenceMeters.COM3003, "voltageAux")]
    [TestCase(VoltageAuxiliaries.VU211x1, CurrentAuxiliaries.VI200x4, ReferenceMeters.COM3003, "currentAux")]
    public async Task Can_Set_Auxiliares(VoltageAuxiliaries voltage, CurrentAuxiliaries current, ReferenceMeters meter, string errorOrResponse)
    {
        if (errorOrResponse.StartsWith("ZP"))
        {
            await Generator.SetAmplifiersAndReferenceMeter(new()
            {
                CurrentAmplifier = CurrentAmplifiers.VUI302,
                CurrentAuxiliary = current,
                ReferenceMeter = meter,
                VoltageAmplifier = VoltageAmplifiers.VUI302,
                VoltageAuxiliary = voltage,
            });

            Assert.That(_port.Commands.Count, Is.EqualTo(1));
            Assert.That(_port.Commands[0], Is.EqualTo(errorOrResponse));
        }
        else
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() => Generator.SetAmplifiersAndReferenceMeter(new()
            {
                CurrentAmplifier = CurrentAmplifiers.VUI302,
                CurrentAuxiliary = current,
                ReferenceMeter = meter,
                VoltageAmplifier = VoltageAmplifiers.VUI302,
                VoltageAuxiliary = voltage,
            }));

            Assert.That(exception.Message, Is.EqualTo(errorOrResponse));
        }
    }
}
