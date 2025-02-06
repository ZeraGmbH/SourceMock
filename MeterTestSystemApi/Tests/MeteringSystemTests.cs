using System.Text.RegularExpressions;
using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions.Device;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using SerialPortProxy;
using SourceApi.Actions.Source;
using ZeraDevices.Source.FG30x;
using ZeraDevices.Source;
using ZeraDevices.Source.MT768;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Provider;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator.FG30x;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator.MT768;
using ZeraDevices.ReferenceMeter.ErrorCalculator;

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

        public void RawWrite(byte[] command) => throw new NotImplementedException();

        public byte? RawRead(int? timeout = null) => throw new NotImplementedException();
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

        Device = SerialPortConnection.FromMockedPortInstance(_port, new NullLogger<ISerialPortConnection>());

        Source = new SerialPortFGSource(_sourceLogger, Device, new CapabilitiesMap(), new SourceCapabilityValidator());

        var services = new ServiceCollection();

        services.AddSingleton(Source);
        services.AddSingleton<ISerialPortFGRefMeter>(new SerialPortFGRefMeter(Device, null!));

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
    public async Task Can_Get_Capabilities_For_FG_Async()
    {
        var caps = await Generator.GetCapabilitiesAsync(new NoopInterfaceLogger());

        Assert.That(caps, Is.Not.Null);
    }

    [Test]
    public async Task Can_Not_Get_Capabilities_For_MT_Async()
    {
        var generator = new SerialPortMTMeterTestSystem(Device,
            new SerialPortMTRefMeter(Device, new NullLogger<SerialPortMTRefMeter>()),
            new SerialPortMTErrorCalculator(Device, new NullLogger<SerialPortMTErrorCalculator>()),
            new NullLogger<SerialPortMTMeterTestSystem>(),
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), Device, new CapabilitiesMap(), new SourceCapabilityValidator()));

        var caps = await generator.GetCapabilitiesAsync(new NoopInterfaceLogger());

        Assert.That(caps, Is.Null);
    }

    [Test]
    public async Task Can_Get_Firmware_Version_For_MT_Async()
    {
        var generator = new SerialPortMTMeterTestSystem(Device,
            new SerialPortMTRefMeter(Device, new NullLogger<SerialPortMTRefMeter>()),
            new SerialPortMTErrorCalculator(Device, new NullLogger<SerialPortMTErrorCalculator>()),
            new NullLogger<SerialPortMTMeterTestSystem>(),
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), Device, new CapabilitiesMap(), new SourceCapabilityValidator()));

        var version = await generator.GetFirmwareVersionAsync(new NoopInterfaceLogger());

        Assert.Multiple(() =>
        {
            Assert.That(version.ModelName, Is.EqualTo("FG399"));
            Assert.That(version.Version, Is.EqualTo("703"));
        });
    }

    [Test]
    public async Task Can_Get_Firmware_Version_For_FG_Async()
    {
        var version = await Generator.GetFirmwareVersionAsync(new NoopInterfaceLogger());

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
    public async Task Can_Set_Amplifiers_Async(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters meter, string errorOrResponse)
    {
        if (errorOrResponse.StartsWith("ZP"))
        {
            await Generator.SetAmplifiersAndReferenceMeterAsync(new NoopInterfaceLogger(), new()
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => Generator.SetAmplifiersAndReferenceMeterAsync(new NoopInterfaceLogger(), new()
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
    public async Task Can_Set_Auxiliares_Async(VoltageAuxiliaries voltage, CurrentAuxiliaries current, ReferenceMeters meter, string errorOrResponse)
    {
        if (errorOrResponse.StartsWith("ZP"))
        {
            await Generator.SetAmplifiersAndReferenceMeterAsync(new NoopInterfaceLogger(), new()
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
            var exception = Assert.ThrowsAsync<ArgumentException>(() => Generator.SetAmplifiersAndReferenceMeterAsync(new NoopInterfaceLogger(), new()
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
