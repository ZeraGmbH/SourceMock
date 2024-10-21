using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZIFApi.Actions;
using ZIFApi.Models;

namespace ZIFApiTests;

[TestFixture]
public class PowerMaster8121MockTests
{

    private IZIFProtocol Socket = null!;

    private ISerialPortConnection Connection = null!;

    [SetUp]
    public void Setup()
    {
        Connection = SerialPortConnection.FromMock<PowerMaster8121SerialPortMock>(new NullLogger<ISerialPortConnection>(), false);

        Socket = new PowerMaster8121(new NullLogger<PowerMaster8121>());
    }

    [TearDown]
    public void Teardown()
    {
        Connection?.Dispose();
    }

    [Test]
    public async Task Can_Read_Software_Version_Async()
    {
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
        var serial = await Socket.GetSerialAsync(Connection, new NoopInterfaceLogger());

        Assert.That(serial, Is.EqualTo(0x6aea));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Can_Activate_Async(bool activate)
    {
        await Socket.SetActiveAsync(activate, Connection, new NoopInterfaceLogger());
    }

    [Test]
    public async Task Can_Read_Status_Async()
    {
        Assert.That(await Socket.GetActiveAsync(Connection, new NoopInterfaceLogger()), Is.EqualTo(true));
        Assert.That(await Socket.GetHasMeterAsync(Connection, new NoopInterfaceLogger()), Is.EqualTo(true));
        Assert.That(await Socket.GetHasErrorAsync(Connection, new NoopInterfaceLogger()), Is.EqualTo(false));
    }

    [Test]
    public async Task Can_Switch_Meter_Async()
    {
        await Socket.SetMeterAsync("8", "4WD", Connection, new NoopInterfaceLogger());
    }
}