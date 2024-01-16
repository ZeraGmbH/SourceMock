using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;

namespace Tests;

/// <summary>
/// Mock able to reply to a single command.
/// </summary>
class PortMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        if (_replies.TryDequeue(out var reply))
            return reply;

        Thread.Sleep(1000);

        throw new TimeoutException("no reply in queue");
    }

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "AAV":
                {
                    _replies.Enqueue("MT786V06.33");
                    _replies.Enqueue("AAVACK");

                    break;
                }
            case "S3CM1":
                {
                    _replies.Enqueue("STUFF");
                    _replies.Enqueue("SOK3CM1");

                    break;
                }
        }
    }
}

[TestFixture]
public class ConnectionTests
{
    private readonly NullLogger<ISerialPortConnection> _logger = new();

    [Test]
    public async Task Can_Read_Firmware_Version()
    {
        using var cut = SerialPortConnection.FromMock<PortMock>(_logger);

        var reply = await cut.Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

        Assert.That(reply.Length, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(reply[0], Is.EqualTo("MT786V06.33"));
            Assert.That(reply[1], Is.EqualTo("AAVACK"));
        });
    }

    [Test]
    public async Task Can_Detect_Parameters_On_Response()
    {
        using var cut = SerialPortConnection.FromMock<PortMock>(_logger);

        var request = SerialPortRequest.Create("S3CM1", new Regex("^SOK3CM([1-4])$"));
        var reply = await cut.Execute(request)[0];

        Assert.That(reply, Has.Length.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(reply[0], Is.EqualTo("STUFF"));
            Assert.That(reply[1], Is.EqualTo("SOK3CM1"));
            Assert.That(request.EndMatch!.Groups[1].Value, Is.EqualTo("1"));
        });
    }
}