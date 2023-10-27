using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Constraints;
using RefMeterApi.Actions.Device;
using SerialPortProxy;

namespace RefMeterApiTests;

class ReplyMock : ISerialPort
{
    private readonly Queue<string> _queue = new();

    private readonly string[] _replies;

    public ReplyMock(params string[] replies)
    {
        _replies = replies;
    }

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
        switch (command)
        {
            case "AME":
                Array.ForEach(this._replies, _queue.Enqueue);

                break;
        }
    }
}

[TestFixture]
public class AMEParserTests
{
    private readonly NullLogger<SerialPortConnection> _logger = new();

    private IRefMeterDevice CreateDevice(params string[] replies) => new SerialPortRefMeterDevice(SerialPortConnection.FromPortInstance(new ReplyMock(replies), _logger));

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

                Assert.That(parsed.PhaseOrder, Is.EqualTo(123));
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
    [TestCase("1;1EA3", typeof(FormatException))]
    [TestCase("12.3;1")]
    [TestCase("xxxx")]
    public void Will_Fail_On_Invalid_Reply(string reply, Type? exception = null)
    {
        Assert.ThrowsAsync(exception ?? typeof(ArgumentException), async () => await CreateDevice(new[] { reply, "AMEACK" }).GetActualValues());
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
}
