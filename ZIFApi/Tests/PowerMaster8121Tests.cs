using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZIFApi.Actions;
using ZIFApi.Exceptions;
using ZIFApi.Models;

namespace ZIFApiTests;

[TestFixture]
public class PowerMaster8121Tests
{
    class PortMock : ISerialPort
    {
        private string _request = "";

        private byte[] _reply = [];

        private Queue<byte> _queue = [];

        public void Register(byte[] request, byte[] reply)
        {
            _request = BitConverter.ToString(request);
            _reply = reply;
            _queue.Clear();
        }

        public void Dispose() { }

        public byte? RawRead(CancellationToken? cancel) => _queue.TryDequeue(out byte data) ? data : null;

        public void RawWrite(byte[] command, CancellationToken? cancel)
        {
            Assert.That(BitConverter.ToString(command), Is.EqualTo(_request));
            Assert.That(_queue, Has.Count.EqualTo(0));

            _queue = new(_reply);
        }

        public string ReadLine(CancellationToken? cancel)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string command, CancellationToken? cancel)
        {
            throw new NotImplementedException();
        }
    }

    private IZIFProtocol Socket = null!;

    private PortMock Port = null!;

    private ISerialPortConnection Connection = null!;

    [SetUp]
    public void Setup()
    {
        Port = new();

        Connection = SerialPortConnection.FromMockedPortInstance(Port, new NullLogger<ISerialPortConnection>(), false);

        Socket = new PowerMaster8121(new PortSetup821xVSW(), new NullLogger<PowerMaster8121>());
    }

    [TearDown]
    public void Teardown()
    {
        Connection?.Dispose();
    }

    [Test]
    public async Task Can_Read_Software_Version_Async()
    {
        Port.Register(
            [0xa5, 0x02, 0xc2, 0xe7, 0x5a],
            [0xa5, 0x08, 0x06, 0xc2, 0x02, 0x00, 0x00, 0x00, 0x16, 0x96, 0x5a]
        );

        var version = await Socket.GetVersionAsync(Connection, new NoopInterfaceLogger());

        Assert.Multiple(() =>
        {
            Assert.That(version.Protocol, Is.EqualTo(SupportedZIFProtocols.PowerMaster8121));
            Assert.That(version.Major, Is.EqualTo(2));
            Assert.That(version.Minor, Is.EqualTo(22));
        });
    }

    [Test]
    public async Task Can_Read_Serial_Number_Async()
    {
        Port.Register(
            [0xA5, 0x02, 0xC1, 0x05, 0x5A],
            [0xA5, 0x05, 0x06, 0xC1, 0x6A, 0xEA, 0x09, 0x5A]
        );

        var serial = await Socket.GetSerialAsync(Connection, new NoopInterfaceLogger());

        Assert.That(serial, Is.EqualTo(0x6aea));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Can_Activate_Async(bool activate)
    {
        Port.Register(
            [0xA5, 0x03, 0x8D, (byte)(activate ? 0x01 : 0x00), (byte)(activate ? 0x1c : 0x42), 0x5A],
            [0xA5, 0x03, 0x06, 0x8D, 0x3F, 0x5A]
        );

        await Socket.SetActiveAsync(activate, Connection, new NoopInterfaceLogger());
    }

    [Test]
    public async Task Can_Read_Status_Async()
    {
        Port.Register(
            [0xA5, 0x02, 0xC4, 0x3A, 0x5A],
            [0xA5, 0x04, 0x06, 0xC4, 0x03, 0xB2, 0x5A]
        );

        Assert.That(await Socket.GetActiveAsync(Connection, new NoopInterfaceLogger()), Is.EqualTo(true));
        Assert.That(await Socket.GetHasMeterAsync(Connection, new NoopInterfaceLogger()), Is.EqualTo(true));
        Assert.That(await Socket.GetHasErrorAsync(Connection, new NoopInterfaceLogger()), Is.EqualTo(false));
    }

    [Test]
    public async Task Can_Switch_Meter_Async()
    {
        Port.Register(
            [0xA5, 0x10, 0x8E, 0x0C, 0x0C, 0x3F, 0x00, 0x3F, 0x02, 0x9F, 0x04, 0xFF, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x56, 0x5A],
            [0xA5, 0x03, 0x06, 0x8E, 0xDD, 0x5A]
        );

        await Socket.SetMeterAsync("8", "4WD", Connection, new NoopInterfaceLogger());
    }

    [Test]
    public void Detect_Bad_Meter_Switch()
    {
        Assert.ThrowsAsync<BadMeterFormServiceTypeCombinationException>(() => Socket.SetMeterAsync("8", "4WY", Connection, new NoopInterfaceLogger())); ;
    }
}