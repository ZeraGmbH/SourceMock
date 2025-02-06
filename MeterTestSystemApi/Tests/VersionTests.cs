using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZeraDevices.Source.MT768;
using ZeraDevices.Source;
using ZERA.WebSam.Shared.Actions;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator.MT768;
using ZeraDevices.ReferenceMeter.ErrorCalculator.MT768;
using ZeraDevices.MeterTestSystem.MT768;
using ZERA.WebSam.Shared.Provider;

namespace MeterTestSystemApiTests;

/// <summary>
/// General mock for validating command reply interpretation.
/// </summary>
abstract class PortMock(string reply) : ISerialPort
{
    private readonly string _reply = reply;
    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public virtual string ReadLine()
    {
        if (_replies.TryDequeue(out var reply))
            return reply;

        Thread.Sleep(100);

        throw new TimeoutException("no reply in queue");
    }

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "AAV":
                {
                    _replies.Enqueue(_reply);
                    _replies.Enqueue("AAVACK");

                    break;
                }
        }
    }

    public void RawWrite(byte[] command) => throw new NotImplementedException();

    public byte? RawRead(int? timeout = null) => throw new NotImplementedException();
}

/// <summary>
/// Mock with a regular esponse.
/// </summary>
class CorrectVersionMock : PortMock
{
    public CorrectVersionMock() : base("MT9123V19.129") { }
}

/// <summary>
/// Mock with a reply missing the separator (V).
/// </summary>
class IncorrectVersionMock : PortMock
{
    public IncorrectVersionMock() : base("MT912319.129") { }
}

/// <summary>
/// Mock creating a NAK reply.
/// </summary>
class NakVersionMock : PortMock
{
    public NakVersionMock() : base("AAVNAK") { }
}

/// <summary>
/// Mock producing a timeout when the reply is requested.
/// </summary>
class VersionTimeoutMock : PortMock
{
    public VersionTimeoutMock() : base("") { }

    public override string ReadLine()
    {
        Thread.Sleep(100);

        throw new TimeoutException("Timed out");
    }
}

/// <summary>
/// Mock with no model name in reply.
/// </summary>
class EmptyModelNameMock : PortMock
{
    public EmptyModelNameMock() : base("V19.129") { }
}

/// <summary>
/// Mock with no version in reply.
/// </summary>
class EmptyVersionMock : PortMock
{
    public EmptyVersionMock() : base("MT9123V") { }
}

[TestFixture]
public class VersionTests
{
    static VersionTests()
    {
        SerialPortConnection.ActivateUnitTestMode(2000);
    }

    private readonly NullLogger<ISerialPortConnection> _logger = new();

    private readonly NullLogger<SerialPortMTMeterTestSystem> _portLogger = new();

    [TestCase(typeof(EmptyModelNameMock), "invalid response V19.129 from device")]
    [TestCase(typeof(EmptyVersionMock), "invalid response MT9123V from device")]
    [TestCase(typeof(IncorrectVersionMock), "invalid response MT912319.129 from device")]
    [TestCase(typeof(NakVersionMock), "AAV", typeof(ArgumentException))]
    [TestCase(typeof(VersionTimeoutMock), "no reply from serial port", typeof(TimeoutException))]
    public void Fails_On_Invalid_Reply(Type mockType, string message, Type? exception = null)
    {
        using var device = SerialPortConnection.FromMock(mockType, _logger);

        var dut = new SerialPortMTMeterTestSystem(device,
            new SerialPortMTRefMeter(device, new NullLogger<SerialPortMTRefMeter>()),
            new SerialPortMTErrorCalculator(device, new NullLogger<SerialPortMTErrorCalculator>()),
            _portLogger,
            new SerialPortMTSource(new NullLogger<SerialPortMTSource>(), device, new CapabilitiesMap(), new SourceCapabilityValidator()));

        var ex = Assert.ThrowsAsync(exception ?? typeof(InvalidOperationException), async () => await dut.GetFirmwareVersionAsync(new NoopInterfaceLogger()));

        Assert.That(ex.Message, Is.EqualTo(message));
    }
}
