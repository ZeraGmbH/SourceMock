using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApiTests.PortMocks;
using SerialPortProxy;

namespace RefMeterApiTests;

class DeviceLogger : ILogger<SerialPortRefMeterDevice>
{
    class Scope : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new Scope();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        throw new ArgumentException(formatter(state, exception));
}

[TestFixture]
public class AMEParserTests
{
    private readonly NullLogger<SerialPortConnection> _portLogger = new();

    private readonly DeviceLogger _deviceLogger = new();

    private IRefMeterDevice CreateDevice(params string[] replies) => new SerialPortRefMeterDevice(SerialPortConnection.FromPortInstance(new FixedReplyMock(replies), _portLogger), _deviceLogger);

    [Test]
    public async Task Can_Parse_AME_Reply()
    {

        var currentCulture = Thread.CurrentThread.CurrentCulture;
        var currentUiCulture = Thread.CurrentThread.CurrentUICulture;

        Thread.CurrentThread.CurrentCulture = new CultureInfo("de");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");

        try
        {
            var device = CreateDevice(File.ReadAllLines(@"TestData/ameReply.txt"));
            var parsed = await device.GetActualValues();

            Assert.Multiple(() =>
            {
                Assert.That(parsed.Frequency, Is.EqualTo(50).Within(0.5));
                Assert.That(parsed.Phases[0].Voltage, Is.EqualTo(20).Within(0.5));
                Assert.That(parsed.Phases[1].Current, Is.EqualTo(0.1).Within(0.05));
                Assert.That(parsed.Phases[1].AngleVoltage, Is.EqualTo(120).Within(0.5));
                Assert.That(parsed.Phases[2].AngleCurrent, Is.EqualTo(240).Within(0.5));

                Assert.That(parsed.PhaseOrder, Is.EqualTo("123"));
            });
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }

    [TestCase("-1;1")]
    [TestCase(";1")]
    [TestCase("1;")]
    [TestCase("1;1EA3")]
    [TestCase("12.3;1")]
    [TestCase("xxxx")]
    public async Task Will_Log_On_Invalid_Reply(string reply)
    {
        /* Use the regular logger. */
        var device = new SerialPortRefMeterDevice(
            SerialPortConnection.FromPortInstance(new FixedReplyMock(new[] { reply, "AMEACK" }), _portLogger),
            new NullLogger<SerialPortRefMeterDevice>()
        );

        /* Bad replies will only log a warning but not throw any exception. */
        await device.GetActualValues();

        /* Each log entry will create an ArgumentException. */
        Assert.ThrowsAsync<ArgumentException>(async () => await CreateDevice(new[] { reply, "AMEACK" }).GetActualValues());
    }

    [Test]
    public async Task Will_Overwrite_Index_Value()
    {
        var data = await CreateDevice(new[] { "28;1", "28;2", "AMEACK" }).GetActualValues();

        Assert.That(data.Frequency, Is.EqualTo(2));
    }

    [Test]
    public async Task Can_Handle_Empty_Reply()
    {
        await CreateDevice(new[] { "AMEACK" }).GetActualValues();

        Assert.Pass();
    }

    [Test]
    public void Will_Detect_Missing_ACK()
    {
        Assert.ThrowsAsync<TimeoutException>(async () => await CreateDevice(new[] { "0;1" }).GetActualValues());
    }

    [Test]
    public async Task Will_Cache_Request()
    {
        var device = new SerialPortRefMeterDevice(SerialPortConnection.FromMock<CountingMock>(_portLogger), _deviceLogger);

        /* Since all task execute at the same time they all should get the same result. */
        var first = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => device.GetActualValues()));

        Array.ForEach(first, r => Assert.That(r.Frequency, Is.EqualTo(50)));

        Thread.Sleep(100);

        /* After all tasks complete a new request is necessary. */
        var second = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => device.GetActualValues()));

        Array.ForEach(second, r => Assert.That(r.Frequency, Is.EqualTo(51)));
    }
}
