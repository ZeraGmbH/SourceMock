using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApiTests;

[TestFixture]
public class RefMeterTests
{
    class MeasuringModesMock : ISerialPort
    {
        private static readonly Regex MaCommand = new(@"^MA(.+)$");

        private static string[] _modes = {
            "1PHA",
            "1PHR",
            "1PHT",
            "3BKB",
            "3LBA",
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
            "4LBG",
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
                case "BU":
                    /* Range Voltage: 250V */
                    _queue.Enqueue("BU250.000");
                    break;
                case "AU":
                    /* Voltage: 199.990V, 249.997V, 299.989V */
                    _queue.Enqueue("AUR15999S20000T23999");
                    break;
                case "BI":
                    // Range Current: 5A */
                    _queue.Enqueue("BI5.000");
                    break;
                case "AI":
                    /* Current: 2A, 1A, 3A */
                    _queue.Enqueue("AIR 8000S 4000T12000");
                    break;
                case "AW":
                    /* Angle (V,A): (0, 5), (110.01, 130), (245, 230.99) */
                    _queue.Enqueue("AWR000.0005.0S110.0130.0T245.0231.0");
                    break;
                case "MP":
                    /* Active Power: 398.014W, 234.899W, 896.617W => 1529.530W */
                    _queue.Enqueue("MPR398.0;S234.9;T896.6");
                    break;
                case "MQ":
                    /* Reactive Power: 34.849var, 85.461var, -78.603var => 41.708var */
                    _queue.Enqueue("MQR34.8;S85.5;T-78.6");
                    break;
                case "MS":
                    /* Apparent power: 400.008VA, 249.968VA, 900.060VA => 1550.032VA */
                    _queue.Enqueue("MSR400.0;S250.0;T900.0");
                    break;
                case "AF":
                    /* Frequency: 50.01Hz */
                    _queue.Enqueue("AF50.01");
                    break;
                default:
                    if (MaCommand.IsMatch(command))
                        _queue.Enqueue("OKMA");

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

    [Test]
    public async Task FG_RefMeter_Can_Set_And_Remeber_Measuring_Mode()
    {
        var device = SerialPortConnection.FromMock<MeasuringModesMock>(new NullLogger<SerialPortConnection>());
        var refMeter = new SerialPortFGRefMeter(device, new NullLogger<SerialPortFGRefMeter>());

        var mode = await refMeter.GetActualMeasurementMode();

        Assert.That(mode, Is.Null);

        await refMeter.SetActualMeasurementMode(MeasurementModes.FourWireApparentPower);

        Assert.That(await refMeter.GetActualMeasurementMode(), Is.EqualTo(MeasurementModes.FourWireApparentPower));
    }

    [Test]
    public async Task FG_RefMeter_Will_Report_Actual_Values()
    {
        var device = SerialPortConnection.FromMock<MeasuringModesMock>(new NullLogger<SerialPortConnection>());
        var refMeter = new SerialPortFGRefMeter(device, new NullLogger<SerialPortFGRefMeter>());

        var values = await refMeter.GetActualValues();

        Assert.Multiple(() =>
        {
            Assert.That(values.Frequency, Is.EqualTo(50.01));
            Assert.That(values.ActivePower, Is.EqualTo(1529.5));
            Assert.That(values.ReactivePower, Is.EqualTo(41.7));
            Assert.That(values.ApparentPower, Is.EqualTo(1550));

            Assert.That(values.Phases[0].ActivePower, Is.EqualTo(398));
            Assert.That(values.Phases[0].ReactivePower, Is.EqualTo(34.8));
            Assert.That(values.Phases[0].ApparentPower, Is.EqualTo(400));
            Assert.That(values.Phases[0].Voltage, Is.EqualTo(199.9875));
            Assert.That(values.Phases[0].AngleVoltage, Is.EqualTo(0));
            Assert.That(values.Phases[0].Current, Is.EqualTo(2));
            Assert.That(values.Phases[0].AngleCurrent, Is.EqualTo(5));
            Assert.That(values.Phases[0].PowerFactor, Is.EqualTo(398.0 / 400.0).Within(0.001));

            Assert.That(values.Phases[1].ActivePower, Is.EqualTo(234.9));
            Assert.That(values.Phases[1].ReactivePower, Is.EqualTo(85.5));
            Assert.That(values.Phases[1].ApparentPower, Is.EqualTo(250));
            Assert.That(values.Phases[1].Voltage, Is.EqualTo(250));
            Assert.That(values.Phases[1].AngleVoltage, Is.EqualTo(110));
            Assert.That(values.Phases[1].Current, Is.EqualTo(1));
            Assert.That(values.Phases[1].AngleCurrent, Is.EqualTo(130));
            Assert.That(values.Phases[1].PowerFactor, Is.EqualTo(234.9 / 250.0).Within(0.001));

            Assert.That(values.Phases[2].ActivePower, Is.EqualTo(896.6));
            Assert.That(values.Phases[2].ReactivePower, Is.EqualTo(-78.6));
            Assert.That(values.Phases[2].ApparentPower, Is.EqualTo(900));
            Assert.That(values.Phases[2].Voltage, Is.EqualTo(299.9875));
            Assert.That(values.Phases[2].AngleVoltage, Is.EqualTo(245));
            Assert.That(values.Phases[2].Current, Is.EqualTo(3));
            Assert.That(values.Phases[2].AngleCurrent, Is.EqualTo(231));
            Assert.That(values.Phases[2].PowerFactor, Is.EqualTo(896.6 / 900.0).Within(0.001));

            Assert.That(values.PhaseOrder, Is.EqualTo("123"));
        });

    }
}
