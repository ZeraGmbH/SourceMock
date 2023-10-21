using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.Device;

namespace WebSamDeviceApis.Tests.Actions.SerialPort;

abstract class PortMock : ISerialPort
{
    private readonly string _reply;

    protected PortMock(string reply)
    {
        _reply = reply;
    }

    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public virtual string ReadLine()
    {
        if (_replies.TryDequeue(out var reply))
            return reply;

        throw new TimeoutException("no reply in quuue");
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
}

class CorrectVersionMock : PortMock
{
    public CorrectVersionMock() : base("MT9123V19.129") { }
}

class IncorrectVersionMock : PortMock
{
    public IncorrectVersionMock() : base("MT912319.129") { }
}

class NakVersionMock : PortMock
{
    public NakVersionMock() : base("AAVNAK") { }
}

class VersionTimeoutMock : PortMock
{
    public VersionTimeoutMock() : base("") { }

    public override string ReadLine() => throw new TimeoutException("Timed out");
}

class EmtpyModelNameMock : PortMock
{
    public EmtpyModelNameMock() : base("V19.129") { }
}

class EmptyVersionMock : PortMock
{
    public EmptyVersionMock() : base("MT9123V") { }
}

[TestFixture]
public class DeviceTests
{
    private readonly NullLogger<SerialPortConnection> _logger = new();

    [Test]
    public async Task Can_Detect_Firmware_Version()
    {
        using var device = SerialPortConnection.FromMock<CorrectVersionMock>(_logger);

        var dut = new SerialPortDevice(device);

        var version = await dut.GetFirmwareVersion();

        Assert.That(version.ModelName, Is.EqualTo("MT9123"));
        Assert.That(version.Version, Is.EqualTo("19.129"));
    }

    [Test]
    public void Fails_On_Invalid_Version()
    {
        using var device = SerialPortConnection.FromMock<IncorrectVersionMock>(_logger);

        var dut = new SerialPortDevice(device);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await dut.GetFirmwareVersion());

        Assert.That(ex.Message, Is.EqualTo("invalid response MT912319.129 from device"));
    }

    [Test]
    public void Fails_On_NAK_Reply()
    {
        using var device = SerialPortConnection.FromMock<NakVersionMock>(_logger);

        var dut = new SerialPortDevice(device);

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await dut.GetFirmwareVersion());

        Assert.That(ex.Message, Is.EqualTo("AAV"));
    }

    [Test]
    public void Fails_On_Communication_Timeout()
    {
        using var device = SerialPortConnection.FromMock<VersionTimeoutMock>(_logger);

        var dut = new SerialPortDevice(device);

        var ex = Assert.ThrowsAsync<TimeoutException>(async () => await dut.GetFirmwareVersion());

        Assert.That(ex.Message, Is.EqualTo("Timed out"));
    }

    [Test]
    public void Fails_On_Empty_Modelname()
    {
        using var device = SerialPortConnection.FromMock<EmtpyModelNameMock>(_logger);

        var dut = new SerialPortDevice(device);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await dut.GetFirmwareVersion());

        Assert.That(ex.Message, Is.EqualTo("invalid response V19.129 from device"));
    }

    [Test]
    public void Fails_On_Empty_Version()
    {
        using var device = SerialPortConnection.FromMock<EmptyVersionMock>(_logger);

        var dut = new SerialPortDevice(device);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await dut.GetFirmwareVersion());

        Assert.That(ex.Message, Is.EqualTo("invalid response MT9123V from device"));
    }
}
