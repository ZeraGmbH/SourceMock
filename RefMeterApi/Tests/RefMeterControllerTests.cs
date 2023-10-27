using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Controllers;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApiTests;

class PortMock : ISerialPort
{
    private static readonly string[] _replies = File.ReadAllLines(@"TestData/ameReply.txt");

    private readonly Queue<string> _queue = new();

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
                Array.ForEach(_replies, _queue.Enqueue);

                break;
        }
    }
}

[TestFixture]
public class RefMeterControllerTests
{
    private readonly NullLogger<SerialPortConnection> _logger = new();

    [Test]
    public async Task Controller_Will_Decode_AME_Reply()
    {
        using var port = SerialPortConnection.FromMock<PortMock>(_logger);

        var cut = new RefMeterController(new SerialPortRefMeterDevice(port));

        var response = await cut.GetCurrentMeasureOutput();
        var result = response.Result as OkObjectResult;
        var data = result?.Value as MeasureOutput;

        Assert.That(data, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(data.Frequency, Is.EqualTo(50).Within(0.5));
            Assert.That(data.Phases[0].Voltage, Is.EqualTo(20).Within(0.5));
            Assert.That(data.Phases[1].Current, Is.EqualTo(0.1).Within(0.05));
            Assert.That(data.Phases[1].AngleVoltage, Is.EqualTo(120).Within(0.5));
            Assert.That(data.Phases[2].AngleCurrent, Is.EqualTo(240).Within(0.5));

            Assert.That(data.PhaseOrder, Is.EqualTo(123));
        });
    }
}
