using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;

namespace Tests;

/// <summary>
/// Mock able to reply to a single command.
/// </summary>
class PortMock : ISerialPort
{
    public readonly Queue<string> Replies = new();

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        if (Replies.TryDequeue(out var reply))
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
                    Replies.Enqueue("MT786V06.33");
                    Replies.Enqueue("AAVACK");

                    break;
                }
            case "S3CM1":
                {
                    Replies.Enqueue("STUFF");
                    Replies.Enqueue("SOK3CM1");

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

        var reply = await cut.CreateExecutor().Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

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
        var reply = await cut.CreateExecutor().Execute(request)[0];

        Assert.That(reply, Has.Length.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(reply[0], Is.EqualTo("STUFF"));
            Assert.That(reply[1], Is.EqualTo("SOK3CM1"));
            Assert.That(request.EndMatch!.Groups[1].Value, Is.EqualTo("1"));
        });
    }

    [Test]
    public void Can_Process_Out_Of_Band_Messages()
    {
        var port = new PortMock();

        using var cut = SerialPortConnection.FromMockedPortInstance(port, _logger);

        var oob = new List<string>();

        cut.RegisterEvent(new Regex(@"^X(\d+)Y$"), m => oob.Add(m.Groups[1].Value));
        cut.RegisterEvent(new Regex(@"^Z$"), m => oob.Add("*"));
        cut.RegisterEvent(new Regex(@"^X12Y$"), m => oob.Add("!"));

        port.Replies.Enqueue("X12Y");
        port.Replies.Enqueue("X1-2Y");
        port.Replies.Enqueue("X13Y");
        port.Replies.Enqueue("Z");
        port.Replies.Enqueue("A");
        port.Replies.Enqueue("Z");

        Thread.Sleep(1500);

        Assert.That(oob, Is.EqualTo(new[] { "!", "12", "13", "*", "*" }));
    }
}
