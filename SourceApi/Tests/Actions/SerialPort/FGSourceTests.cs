using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.SerialPort.FG30x;

namespace WebSamDeviceApis.Tests.Actions.SerialPort;

[TestFixture]
public class FGSourceTests
{
    private readonly NullLogger<SerialPortFGSource> _portLogger = new();

    private readonly NullLogger<SerialPortConnection> _connectionLogger = new();

    private SerialPortConnection _device;

    [SetUp]
    public void SetUp()
    {
        _device = SerialPortConnection.FromMock<SerialPortFGMock>(_connectionLogger);
    }

    [TearDown]
    public void TearDown()
    {
        _device?.Dispose();
    }

    [Test]
    public async Task Can_Get_Firmware_Version()
    {
        var sut = new SerialPortFGSource(_portLogger, _device);

        var version = await sut.GetFirmwareVersion();

        Assert.Multiple(() =>
        {
            Assert.That(version.ModelName, Is.EqualTo("FG399"));
            Assert.That(version.Version, Is.EqualTo("V703"));
        });
    }
}
