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

    [Test]
    public async Task Can_Measure_Async()
    {
        var values = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create("ME", "MEACK")));

        Assert.That(values[0], Has.Length.EqualTo(7));
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
    public async Task Validate_Multiple_Steps_Async()
    {
        var burdens = await Task.WhenAll(
           Connection
               .CreateExecutor(InterfaceLogSourceTypes.Burden)
               .ExecuteAsync(Logger, SerialPortRequest.Create("AB", "ABACK")));

        var all = new List<string>();

        foreach (var burden in burdens[0])
            if (burden != "ABACK")
            {
                var ranges = await Task.WhenAll(
                    Connection
                        .CreateExecutor(InterfaceLogSourceTypes.Burden)
                        .ExecuteAsync(Logger, SerialPortRequest.Create($"AR{burden}", "ARACK")));

                var calibrations = await Task.WhenAll(
                    Connection
                        .CreateExecutor(InterfaceLogSourceTypes.Burden)
                        .ExecuteAsync(Logger, SerialPortRequest.Create($"AN{burden}", "ANACK")));

                foreach (var range in ranges[0])
                    if (range != "ARACK")
                        foreach (var calibration in calibrations[0])
                            if (calibration != "ANACK")
                                all.Add($"{burden};{range};{calibration}");
            }

        Assert.That(all, Has.Count.EqualTo(1239));

        for (var i = 20; i-- > 0;)
        {
            var index = Random.Shared.Next(0, all.Count);

            var step = await Task.WhenAll(
                Connection
                    .CreateExecutor(InterfaceLogSourceTypes.Burden)
                    .ExecuteAsync(Logger, SerialPortRequest.Create($"GA{all[index]}", "GAACK")));

            Assert.That(step[0], Has.Length.EqualTo(2));

            all.RemoveAt(index);
        }
    }

    [TestCase(0)]
    [TestCase(1)]
    public async Task Can_Switch_On_And_Off_Async(int what)
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"ON{what}", "ONACK")));
    }

    [Test]
    public void Can_Not_Switch_On_And_Off()
    {
        Assert.ThrowsAsync<ArgumentException>(() => Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"ON2", "ONACK"))));
    }

    [TestCase("IEC50")]
    [TestCase("IEC60")]
    [TestCase("ANSI")]
    public async Task Can_Set_Burden_Async(string burden)
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SB{burden}", "SBACK")));
    }

    [Test]
    public void Can_Not_Set_Burden()
    {
        Assert.ThrowsAsync<ArgumentException>(() => Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SBIEC75", "SBACK"))));
    }

    [TestCase("IEC50", "100")]
    [TestCase("IEC60", "230/3")]
    [TestCase("ANSI", "110/v3")]
    public async Task Can_Set_Range_Async(string burden, string range)
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SB{burden}", "SBACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SR{range}", "SRACK")));
    }

    [Test]
    public async Task Can_Not_Set_Range_Async()
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SBANSI", "SBACK")));

        try
        {
            await Task.WhenAll(
               Connection
                   .CreateExecutor(InterfaceLogSourceTypes.Burden)
                   .ExecuteAsync(Logger, SerialPortRequest.Create($"SR999", "SRACK")));
        }
        catch (ArgumentException)
        {
        }
    }

    [TestCase("IEC50", "75.00;0.80")]
    [TestCase("IEC60", "18.75;0.80")]
    [TestCase("ANSI", "75.00;0.85")]
    public async Task Can_Set_Step_Async(string burden, string step)
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SB{burden}", "SBACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SN{step}", "SNACK")));
    }

    [TestCase("IEC60", "230/3", "18.75;0.80", "0x12;0x34;0x7f;0x6e")]
    public async Task Can_Set_Transient_Calibration_Async(string burden, string range, string step, string calibration)
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SB{burden}", "SBACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SR{range}", "SRACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SN{step}", "SNACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SF{calibration}", "SFACK")));
    }

    [TestCase("IEC60", "230/3", "18.75;0.80", "0x12;0x34;0x7f;0x6e")]
    public async Task Can_Set_Permanent_Calibration_Async(string burden, string range, string step, string calibration)
    {
        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SA{burden};{range};{step};{calibration};0.0", "SAACK")));

        var values = await Task.WhenAll(
           Connection
               .CreateExecutor(InterfaceLogSourceTypes.Burden)
               .ExecuteAsync(Logger, SerialPortRequest.Create($"GA{burden};{range};{step}", "GAACK")));

        Assert.That(values[0], Is.EqualTo(new string[] { "1;" + calibration, "GAACK" }));
    }

    [TestCase("IEC60", "190", "3.75;0.80")]
    public async Task Can_ReadStatus(string burden, string range, string step)
    {
        var status = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create("ST", "STACK")));

        Assert.That(status[0], Is.EqualTo(new string[] { "B:IEC50", "R:230", "N:0.00;0.00", "ON:0", "STACK" }));

        await Task.WhenAll(
                   Connection
                       .CreateExecutor(InterfaceLogSourceTypes.Burden)
                       .ExecuteAsync(Logger, SerialPortRequest.Create($"SB{burden}", "SBACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SR{range}", "SRACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"SN{step}", "SNACK")));

        await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create($"ON1", "ONACK")));

        status = await Task.WhenAll(
            Connection
                .CreateExecutor(InterfaceLogSourceTypes.Burden)
                .ExecuteAsync(Logger, SerialPortRequest.Create("ST", "STACK")));

        Assert.That(status[0], Is.EqualTo(new string[] { $"B:{burden}", $"R:{range}", $"N:{step}", "ON:1", "STACK" }));
    }
}