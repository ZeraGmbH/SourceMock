using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator.MT768;
using ZeraDeviceTests.PortMocks;

namespace ZeraDeviceTests.ReferenceMeter;

[TestFixture]
public class AMLParserTests
{
    private readonly NullLogger<ISerialPortConnection> _portLogger = new();

    [TestCase("2WA", MeasurementModes.TwoWireActivePower)]
    [TestCase("3WA", MeasurementModes.ThreeWireActivePower)]
    [TestCase("4WA", MeasurementModes.FourWireActivePower)]
    public async Task Can_Process_Supported_Modes_Async(string modeAsString, MeasurementModes mode)
    {
        var device = new SerialPortMTRefMeter(
            SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(new[] { $"01;{modeAsString};xxx", "AMLACK" }), _portLogger),
            new NullLogger<SerialPortMTRefMeter>()
        );

        var modes = await device.GetMeasurementModesAsync(new NoopInterfaceLogger());

        Assert.That(modes.Length, Is.EqualTo(1));
        Assert.That(modes[0], Is.EqualTo(mode));
    }

    [TestCase("01;2WA")]
    [TestCase("xxx;2WA;yyy")]
    [TestCase("")]
    [TestCase("01;XXX;ZZZ")]
    public async Task Will_Discard_Bad_Input_Async(string reply)
    {
        var device = new SerialPortMTRefMeter(
            SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(new[] {
                "01;2WA;2WAde",
                reply,
                "03;3WA;3WAde",
                "AMLACK"
            }), _portLogger),
            new NullLogger<SerialPortMTRefMeter>()
        );

        var modes = await device.GetMeasurementModesAsync(new NoopInterfaceLogger());

        Assert.That(modes.Length, Is.EqualTo(2));
    }

    [TestCase("M=2WA", MeasurementModes.TwoWireActivePower)]
    [TestCase("M=3WA", MeasurementModes.ThreeWireActivePower)]
    [TestCase("M=4WA", MeasurementModes.FourWireActivePower)]
    public async Task Can_Detect_Active_Mode_Async(string modeAsString, MeasurementModes mode)
    {
        var device = new SerialPortMTRefMeter(
            SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(new[] { modeAsString, "ASTACK" }), _portLogger),
            new NullLogger<SerialPortMTRefMeter>()
        );

        var reply = await device.GetActualMeasurementModeAsync(new NoopInterfaceLogger());

        Assert.That(reply, Is.EqualTo(mode));
    }

    [TestCase("XX=12")]
    [TestCase("M=")]
    [TestCase("M=JOJO")]
    public async Task Will_Detect_Unsupported_Mode_Async(string reply)
    {
        var device = new SerialPortMTRefMeter(
            SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(new[] {
                "A=1",
                reply,
                "B=2",
                "ASTACK"
            }), _portLogger),
            new NullLogger<SerialPortMTRefMeter>()
        );

        var mode = await device.GetActualMeasurementModeAsync(new NoopInterfaceLogger());

        Assert.That(mode, Is.Null);
    }
}

