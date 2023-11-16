using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApiTests;

[TestFixture]
public class ErrorMeasurementTests
{
    class PortMock : ISerialPort
    {
        private readonly Queue<string> _queue = new();

        public string[] StatusResponse = { };

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
                case "AEB0":
                case "AEB1":
                    _queue.Enqueue("AEBACK");
                    break;
                case "AEE":
                    _queue.Enqueue("AEEACK");
                    break;
                case "AES1":
                    Array.ForEach(StatusResponse, r => _queue.Enqueue(r));

                    _queue.Enqueue("AESACK");
                    break;
            }
        }

    }

    private readonly NullLogger<SerialPortRefMeterDevice> _logger = new();

    private readonly PortMock _port = new();

    private SerialPortConnection Device = null!;

    [SetUp]
    public void Setup()
    {
        Device = SerialPortConnection.FromPortInstance(_port, new NullLogger<SerialPortConnection>());
    }

    [Test]
    public async Task Can_Start_Error_Measure()
    {
        var cut = new SerialPortRefMeterDevice(Device, _logger);

        await cut.StartErrorMeasurement(false);
        await cut.StartErrorMeasurement(true);
    }

    [Test]
    public async Task Can_Abort_Error_Measure()
    {
        var cut = new SerialPortRefMeterDevice(Device, _logger);

        await cut.AbortErrorMeasurement();
    }

    [Test]
    public async Task Can_Request_Error_Management_Status()
    {
        var cut = new SerialPortRefMeterDevice(Device, _logger);

        var status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.NotActive));
            Assert.That(status.Energy, Is.Null);
            Assert.That(status.Progress, Is.Null);
            Assert.That(status.ErrorValue, Is.Null);
        });

        _port.StatusResponse = new string[] { "00", "oo.oo", "0.000000;0.000000" };

        status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.NotActive));
            Assert.That(status.ErrorValue, Is.Null);
            Assert.That(status.Progress, Is.EqualTo(0d));
            Assert.That(status.Energy, Is.EqualTo(0d));
        });

        _port.StatusResponse = new string[] { "11", "--.--", "0.000000;0.000000" };

        status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.Active));
            Assert.That(status.ErrorValue, Is.Null);
            Assert.That(status.Progress, Is.EqualTo(0d));
            Assert.That(status.Energy, Is.EqualTo(0d));
        });

        _port.StatusResponse = new string[] { "12", "--.--", "51.234112;912.38433" };

        status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.Run));
            Assert.That(status.ErrorValue, Is.Null);
            Assert.That(status.Progress, Is.EqualTo(51.234112d));
            Assert.That(status.Energy, Is.EqualTo(912.38433d));
        });

        _port.StatusResponse = new string[] { "13", "0.5", "51.234112;912.38433" };

        status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.Finished));
            Assert.That(status.ErrorValue, Is.EqualTo(0.5d));
            Assert.That(status.Progress, Is.EqualTo(51.234112d));
            Assert.That(status.Energy, Is.EqualTo(912.38433d));
        });

        _port.StatusResponse = new string[] { "13", "-0.2", "51.234112;912.38433" };

        status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.Finished));
            Assert.That(status.ErrorValue, Is.EqualTo(-0.2d));
            Assert.That(status.Progress, Is.EqualTo(51.234112d));
            Assert.That(status.Energy, Is.EqualTo(912.38433d));
        });

        _port.StatusResponse = new string[] { "03", "-.2", "51.234112;912.38433" };

        status = await cut.GetErrorStatus();

        Assert.That(status, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(status.State, Is.EqualTo(ErrorMeasurementStates.Finished));
            Assert.That(status.ErrorValue, Is.EqualTo(-0.2d));
            Assert.That(status.Progress, Is.EqualTo(51.234112d));
            Assert.That(status.Energy, Is.EqualTo(912.38433d));
        });
    }
}