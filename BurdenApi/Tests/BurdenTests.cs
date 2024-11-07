using BurdenApi.Actions;
using BurdenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZstdSharp.Unsafe;

namespace BurdenApiTests;

[TestFixture]
public class BurdenTests
{
    private readonly IInterfaceLogger Logger = new NoopInterfaceLogger();

    private ServiceProvider Services = null!;

    private IBurden Burden = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        var device = SerialPortConnection.FromMock<BurdenSerialPortMock>(new NullLogger<SerialPortConnection>());

        services.AddKeyedSingleton("Burden", device);

        services.AddTransient<IBurden, Burden>();

        Services = services.BuildServiceProvider();

        Burden = Services.GetRequiredService<IBurden>();
    }

    [TearDown]
    public void TearDown()
    {
        Services?.Dispose();
    }

    [Test]
    public async Task Can_Retrieve_Version_Async()
    {
        var version = await Burden.GetVersionAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(version.Version, Is.EqualTo("EBV33.12"));
            Assert.That(version.Supplement, Is.EqualTo("XB"));
        });
    }

    [TestCase("IEC60", "190", "3.75;0.80", 37, 25, 55, 65)]
    public async Task Can_Read_Calibration_Async(string burden, string range, string step, byte rCoarse, byte rFine, byte lCoarse, byte lFine)
    {
        var calibration = await Burden.GetCalibrationAsync(burden, range, step, Logger);

        Assert.That(calibration, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(calibration.Resistive.Coarse, Is.EqualTo(rCoarse));
            Assert.That(calibration.Resistive.Fine, Is.EqualTo(rFine));
            Assert.That(calibration.Inductive.Coarse, Is.EqualTo(lCoarse));
            Assert.That(calibration.Inductive.Fine, Is.EqualTo(lFine));
        });
    }

    [TestCase("IEC50", "230", "0.00;0.00")]
    public async Task Can_Read_No_Calibration_Async(string burden, string range, string step)
    {
        var calibration = await Burden.GetCalibrationAsync(burden, range, step, Logger);

        Assert.That(calibration, Is.Null);
    }

    [Test]
    public async Task Can_Get_Burdens_Async()
    {
        var version = await Burden.GetBurdensAsync(Logger);

        Assert.That(version, Is.EqualTo(new string[] { "ANSI", "IEC50", "IEC60" }));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task Can_Set_Burden_Activation_Async(bool on)
    {
        await Burden.SetActiveAsync(on, Logger);

        var status = await Burden.GetStatusAsync(Logger);

        Assert.That(status.Active, Is.EqualTo(on));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("ANSI")]
    [Ignore("Takes a minute per test to complete - better run manually only")]
    public async Task Can_Program_Burden_Async(string? burden)
    {
        await Burden.ProgramAsync(burden, Logger);
    }

    [Test]
    public void Can_Not_Program_Burden()
    {
        Assert.ThrowsAsync<ArgumentException>(() => Burden.ProgramAsync("IEC75", Logger));
    }

    [Test]
    public async Task Can_Get_Status_Async()
    {
        var status = await Burden.GetStatusAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(status.Active, Is.False);
            Assert.That(status.Burden, Is.EqualTo("IEC50"));
            Assert.That(status.Range, Is.EqualTo("230"));
            Assert.That(status.Step, Is.EqualTo("0.00;0.00"));
        });

        await Burden.SetBurdenAsync("ANSI", Logger);

        status = await Burden.GetStatusAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(status.Active, Is.False);
            Assert.That(status.Burden, Is.EqualTo("ANSI"));
            Assert.That(status.Range, Is.EqualTo("230"));
            Assert.That(status.Step, Is.EqualTo("12.50;0.10"));
        });

        await Burden.SetRangeAsync("100/v3", Logger);

        status = await Burden.GetStatusAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(status.Active, Is.False);
            Assert.That(status.Burden, Is.EqualTo("ANSI"));
            Assert.That(status.Range, Is.EqualTo("100/v3"));
            Assert.That(status.Step, Is.EqualTo("12.50;0.10"));
        });

        await Burden.SetStepAsync("200.00;0.85", Logger);

        status = await Burden.GetStatusAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(status.Active, Is.False);
            Assert.That(status.Burden, Is.EqualTo("ANSI"));
            Assert.That(status.Range, Is.EqualTo("100/v3"));
            Assert.That(status.Step, Is.EqualTo("200.00;0.85"));
        });

        await Burden.SetActiveAsync(true, Logger);

        status = await Burden.GetStatusAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(status.Active, Is.True);
            Assert.That(status.Burden, Is.EqualTo("ANSI"));
            Assert.That(status.Range, Is.EqualTo("100/v3"));
            Assert.That(status.Step, Is.EqualTo("200.00;0.85"));
        });
    }


    [TestCase("IEC60", "190", "3.75;0.80", 10, 20, 30, 40)]
    public async Task Can_Set_Transient_Calibration_Async(string burden, string range, string step, byte rCoarse, byte rFine, byte lCoarse, byte lFine)
    {
        var before = await Burden.GetCalibrationAsync(burden, range, step, Logger);

        Assert.That(before, Is.Not.Null);

        await Burden.SetBurdenAsync(burden, Logger);
        await Burden.SetRangeAsync(range, Logger);
        await Burden.SetStepAsync(step, Logger);

        var calibration = new Calibration(new(rCoarse, rFine), new(lCoarse, lFine));

        await Burden.SetTransientCalibrationAsync(calibration, Logger);

        var after = await Burden.GetCalibrationAsync(burden, range, step, Logger);

        Assert.That(after, Is.EqualTo(before));
    }

    [TestCase("IEC60", "190", "3.75;0.80", 10, 20, 30, 40)]
    public async Task Can_Set_Permanent_Calibration_Async(string burden, string range, string step, byte rCoarse, byte rFine, byte lCoarse, byte lFine)
    {
        var calibration = new Calibration(new(rCoarse, rFine), new(lCoarse, lFine));

        await Burden.SetPermanentCalibrationAsync(burden, range, step, calibration, Logger);

        var after = await Burden.GetCalibrationAsync(burden, range, step, Logger);

        Assert.That(after, Is.EqualTo(calibration));
    }

    [Test]
    public async Task Can_Measure_Async()
    {
        var values = await Burden.MeasureAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That((double)values.Voltage, Is.EqualTo(109.70).Within(0.01));
            Assert.That((double)values.Current, Is.EqualTo(0.009).Within(0.001));
            Assert.That((double)values.Angle, Is.EqualTo(0.71).Within(0.01));
            Assert.That((double)values.PowerFactor, Is.EqualTo(0.999924).Within(0.0000001));
            Assert.That((double)values.ApparentPower, Is.EqualTo(0.994).Within(0.001));
        });
    }
}