using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.SerialPort.FG30x;

namespace WebSamDeviceApis.Tests.Actions.SerialPort;

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

    private readonly NullLogger<SerialPortConnection> _connectionLogger = new();

    private SerialPortConnection _device;

    private SerialPortFGSource _source;

    private readonly LoggingSourceMock _port = new();

    [SetUp]
    public void SetUp()
    {
        _device = SerialPortConnection.FromPortInstance(_port, _connectionLogger);
        _source = new SerialPortFGSource(_portLogger, _device);
    }

    [TearDown]
    public void TearDown()
    {
        _device?.Dispose();
    }

    [Test]
    public async Task Can_Get_Firmware_Version()
    {
        var version = await _source.GetFirmwareVersion();

        Assert.Multiple(() =>
        {
            Assert.That(version.ModelName, Is.EqualTo("FG399"));
            Assert.That(version.Version, Is.EqualTo("V703"));
        });
    }
}
