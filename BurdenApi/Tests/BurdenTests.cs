using BurdenApi.Actions;
using BurdenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;

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
    }
}