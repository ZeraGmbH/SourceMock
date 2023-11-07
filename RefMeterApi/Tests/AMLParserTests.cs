using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using RefMeterApiTests.PortMocks;
using SerialPortProxy;

namespace RefMeterApiTests;

[TestFixture]
public class AMLParserTests
{
    private readonly NullLogger<SerialPortConnection> _portLogger = new();

    private readonly DeviceLogger _deviceLogger = new();

    [TestCase("2WA", MeasurementModes.TwoWireActivePower)]
    [TestCase("3WA", MeasurementModes.ThreeWireActivePower)]
    [TestCase("4WA", MeasurementModes.FourWireActivePower)]
    public async Task Can_Process_Supported_Modes(string modeAsString, MeasurementModes mode)
    {
        var device = new SerialPortRefMeterDevice(
            SerialPortConnection.FromPortInstance(new FixedReplyMock(new[] { $"01;{modeAsString};xxx", "AMLACK" }), _portLogger),
            new NullLogger<SerialPortRefMeterDevice>()
        );

        var modes = await device.GetMeasurementModes();

        Assert.That(modes.Length, Is.EqualTo(1));
        Assert.That(modes[0], Is.EqualTo(mode));
    }

    [TestCase("01;2WA")]
    [TestCase("xxx;2WA;yyy")]
    [TestCase("")]
    [TestCase("01;XXX;ZZZ")]
    public async Task Will_Discard_Bad_Input(string reply)
    {
        var device = new SerialPortRefMeterDevice(
            SerialPortConnection.FromPortInstance(new FixedReplyMock(new[] {
                "01;2WA;2WAde",
                reply,
                "03;3WA;3WAde",
                "AMLACK"
            }), _portLogger),
            new NullLogger<SerialPortRefMeterDevice>()
        );

        var modes = await device.GetMeasurementModes();

        Assert.That(modes.Length, Is.EqualTo(2));
    }
}
