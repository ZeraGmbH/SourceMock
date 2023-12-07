using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using SerialPortProxy;

namespace RefMeterApiTests;

[TestFixture]
public class RefMeterTests
{
    class MeasuringModesMock : ISerialPort
    {
        private static string[] _modes = {
            "1PHA",
            "1PHR",
            "1PHT",
            "3BKB",
            "3LBA",// Unknwon
            "3LBE",
            "3LBG",
            "3LBK",
            "3LQ6",
            "3LS",
            "3LSG",
            "3LW",
            "3LWR",
            "3Q6K",
            "4LBE",
            "4LBF",
            "4LBG",// Unknown
            "4LBK",
            "4LQ6",
            "4LS",
            "4LSG",
            "4LW",
            "4Q6K",
        };

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
                case "MI":
                    _queue.Enqueue($"MI{string.Join(";", _modes)};");
                    break;
            }
        }
    }

    [Test]
    public async Task FG_RefMeter_Will_Report_List_Of_Measuring_Modes()
    {
        var device = SerialPortConnection.FromMock<MeasuringModesMock>(new NullLogger<SerialPortConnection>());
        var refMeter = new SerialPortFGRefMeter(device, new NullLogger<SerialPortFGRefMeter>());

        var modes = await refMeter.GetMeasurementModes();

        Assert.That(modes, Has.Length.EqualTo(9));
    }
}
