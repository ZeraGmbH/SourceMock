using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApiTests.PortMocks;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;

namespace RefMeterApiTests;

[TestFixture]
public class AMEParserTests
{
    static AMEParserTests()
    {
        SerialPortConnection.ActivateUnitTestMode(2000);
    }

    private readonly NullLogger<ISerialPortConnection> _portLogger = new();

    private readonly DeviceLogger _deviceLogger = new();

    private IRefMeter CreateDevice(params string[] replies) => new SerialPortMTRefMeter(SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(replies), _portLogger), _deviceLogger);

    [Test]
    public async Task Can_Parse_AME_Reply_Async()
    {
        var ameResponse = new List<string>(File.ReadAllLines(@"TestData/ameReply.txt"));

        ameResponse.Insert(0, "ATIACK");

        var device = CreateDevice(ameResponse.ToArray());
        var parsed = await device.GetActualValuesAsync(new NoopInterfaceLogger());

        Assert.Multiple(() =>
        {
            Assert.That((double?)parsed.Frequency, Is.EqualTo(50).Within(0.5));
            Assert.That((double)parsed.Phases[0].Voltage.AcComponent!.Rms, Is.EqualTo(20).Within(0.5));
            Assert.That((double)parsed.Phases[1].Current.AcComponent!.Rms, Is.EqualTo(0.1).Within(0.05));
            Assert.That((double)parsed.Phases[1].Voltage.AcComponent!.Angle, Is.EqualTo(240).Within(0.5));
            Assert.That((double)parsed.Phases[2].Current.AcComponent!.Angle, Is.EqualTo(120).Within(0.5));
            Assert.That((double?)parsed.Phases[0].PowerFactor, Is.EqualTo(0.99965084));

            Assert.That(parsed.PhaseOrder, Is.EqualTo("123"));
        });
    }

    [TestCase("-1;1")]
    [TestCase(";1")]
    [TestCase("1;")]
    [TestCase("1;1EA3")]
    [TestCase("12.3;1")]
    [TestCase("xxxx")]
    public async Task Will_Log_On_Invalid_Reply_Async(string reply)
    {
        /* Use the regular logger. */
        var device = new SerialPortMTRefMeter(
            SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(["ATIACK", reply, "AMEACK"]), _portLogger),
            new NullLogger<SerialPortMTRefMeter>()
        );

        /* Bad replies will only log a warning but not throw any exception. */
        await device.GetActualValuesAsync(new NoopInterfaceLogger());

        /* Each log entry will create an ArgumentException. */
        Assert.ThrowsAsync<ArgumentException>(async () => await CreateDevice(["ATIACK", reply, "AMEACK"]).GetActualValuesAsync(new NoopInterfaceLogger()));
    }

    [Test]
    public async Task Will_Overwrite_Index_Value_Async()
    {
        var data = await CreateDevice(["ATIACK", "28;1", "28;2", "AMEACK"]).GetActualValuesAsync(new NoopInterfaceLogger());

        Assert.That((double?)data.Frequency, Is.EqualTo(2));
    }

    [Test]
    public async Task Can_Handle_Empty_Reply_Async()
    {
        await CreateDevice(["ATIACK", "AMEACK"]).GetActualValuesAsync(new NoopInterfaceLogger());

        Assert.Pass();
    }

    [Test]
    public void Will_Detect_Missing_ACK()
    {
        Assert.ThrowsAsync<TimeoutException>(async () => await CreateDevice(["ATIACK", "0;1"]).GetActualValuesAsync(new NoopInterfaceLogger()));
    }

    [Test]
    public async Task Will_Cache_Request_Async()
    {
        var device = new SerialPortMTRefMeter(SerialPortConnection.FromMock<CountingMock>(_portLogger), _deviceLogger);

        /* Since all task execute at the same time they all should get the same result. */
        var first = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => device.GetActualValuesAsync(new NoopInterfaceLogger())));

        Array.ForEach(first, r => Assert.That((double?)r.Frequency, Is.EqualTo(50)));

        await Task.Delay(2000);

        /* After all tasks complete a new request is necessary. */
        var second = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => device.GetActualValuesAsync(new NoopInterfaceLogger())));

        Array.ForEach(second, r => Assert.That((double?)r.Frequency, Is.EqualTo(51)));
    }
}
