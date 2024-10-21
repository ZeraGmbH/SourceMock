using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZIFApi.Actions;
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

        public byte? RawRead() => _queue.TryDequeue(out byte data) ? data : null;

        public void RawWrite(byte[] command)
        {
            Assert.That(BitConverter.ToString(command), Is.EqualTo(_request));
            Assert.That(_queue, Has.Count.EqualTo(0));

            _queue = new(_reply);
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string command)
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

        Socket = new PowerMaster8121(new NullLogger<PowerMaster8121>());
    }

    [TearDown]
    public void Teardown()
    {
        Connection?.Dispose();
    }

    [Test]
    public async Task Can_Read_Software_Version()
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
}