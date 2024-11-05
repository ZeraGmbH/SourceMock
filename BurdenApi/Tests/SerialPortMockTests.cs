using BurdenApi.Actions;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApiTests;

[TestFixture]
public class SerialPortMockTests
{
    private readonly IInterfaceLogger Logger = new NoopInterfaceLogger();

    private ISerialPortConnection Connection = null!;

    [SetUp]
    public void Setup()
    {
        Connection = SerialPortConnection.FromMock<BurdenSerialPortMock>(new NullLogger<SerialPortConnection>());
    }

    [TearDown]
    public void Teardown()
    {
        Connection?.Dispose();
    }

    [Test]
    public async Task Can_Get_Version_Async()
    {
        var version = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create("AV", "AVACK")));

        Assert.That(version[0], Is.EqualTo(new string[] { "EBV33.12", "XB", "AVACK" }));
    }

    [Test]
    public async Task Can_Get_Burdens_Async()
    {
        var burdens = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create("AB", "ABACK")));

        Assert.That(burdens[0], Is.EqualTo(new string[] { "ANSI", "IEC50", "IEC60", "ABACK" }));
    }

    [TestCase("IEC50")]
    [TestCase("IEC60")]
    [TestCase("ANSI")]
    public async Task Can_Get_Burden_Ranges_Async(string burden)
    {
        var ranges = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"AR{burden}", "ARACK")));

        Assert.That(ranges[0], Has.Length.EqualTo(22));
        Assert.That(ranges[0], Contains.Item("100/v3"));
    }


    [TestCase("IEC50", 28, "1.25;0.80")]
    [TestCase("IEC60", 28, "1.25;0.80")]
    [TestCase("ANSI", 6, "25.00;0.70")]
    public async Task Can_Get_Burden_Steps_Async(string burden, int count, string sample)
    {
        var calibrations = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"AN{burden}", "ANACK")));

        Assert.That(calibrations[0], Has.Length.EqualTo(count));
        Assert.That(calibrations[0], Contains.Item(sample));
    }

    [TestCase("IEC50;200;0.00;0.00", "0")]
    [TestCase("IEC60;230/3;0.00;0.00", "1;0x0;0x0;0x0;0x0;0.000000")]
    [TestCase("ANSI;110;200.00;0.85", "1;0x67;0x32;0x6B;0x28;0.000000")]
    public async Task Can_Get_Calibration_Async(string step, string calibration)
    {
        var values = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"GA{step}", "GAACK")));

        Assert.That(values[0], Is.EqualTo(new string[] { calibration, "GAACK" }));
    }

    [Test]
    public async Task Validate_All_Steps_Async()
    {
        var burdens = await Task.WhenAll(
           Connection
               .CreateExecutor(InterfaceLogSourceTypes.Burden)
               .ExecuteAsync(Logger, SerialPortRequest.Create("AB", "ABACK")));

        foreach (var burden in burdens[0])
        {
            var ranges = await Task.WhenAll(
                Connection
                    .CreateExecutor(InterfaceLogSourceTypes.Burden)
                    .ExecuteAsync(Logger, SerialPortRequest.Create($"AR{burden}", "ARACK")));

            var calibrations = await Task.WhenAll(
                Connection
                    .CreateExecutor(InterfaceLogSourceTypes.Burden)
                    .ExecuteAsync(Logger, SerialPortRequest.Create($"AN{burden}", "ANACK")));

            foreach (var range in ranges)
                foreach (var calibration in calibrations)
                {
                    var step = await Task.WhenAll(
                        Connection
                            .CreateExecutor(InterfaceLogSourceTypes.Burden)
                            .ExecuteAsync(Logger, SerialPortRequest.Create($"GA{range};{calibration}", "GAACK")));

                    Assert.That(step[0], Has.Length.EqualTo(2));
                }
        }
    }
}