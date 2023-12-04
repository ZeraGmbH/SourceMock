using System.Text.RegularExpressions;
using MeteringSystemApi.Actions.Device;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Models;
using SerialPortProxy;
using SourceApi.Model;

namespace MeteringSystemApiTests;

[TestFixture]
public class GeneratorTests
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
            if (ZpCommand.IsMatch(command))
                _queue.Enqueue("OKZP");
        }
    }

    private readonly NullLogger<SerialPortFGMeteringSystem> _logger = new();

    private PortMock _port = null!;

    private ISerialPortConnection Device = null!;

    private IMeteringSystem Generator = null!;

    [SetUp]
    public void Setup()
    {
        _port = new();

        Device = SerialPortConnection.FromPortInstance(_port, new NullLogger<ISerialPortConnection>());

        Generator = new SerialPortFGMeteringSystem(Device, _logger);
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
        var generator = new SerialPortMTMeteringSystem();

        var caps = await generator.GetCapabilities();

        Assert.That(caps, Is.Null);
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
