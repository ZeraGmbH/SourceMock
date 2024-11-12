using System.Diagnostics.CodeAnalysis;
using BurdenApi.Actions.Device;
using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApiTests;

[TestFixture]
public class BurdenHardwareTests
{
    protected ServiceProvider Services = null!;

    protected ICalibrationHardware Hardware = null!;

    protected Voltage? VoltageRange;

    protected Current? CurrentRange;

    protected PllChannel? PLL;

    protected bool? AutomaticVoltage;

    protected bool? AutomaticCurrent;

    protected bool? AutomaticPLL;

    protected MeasurementModes? MeasurementMode;

    protected bool IsVoltage;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddTransient<ICalibrationHardware, CalibrationHardware>();
        services.AddTransient<IInterfaceLogger, NoopInterfaceLogger>();

        var source = new Mock<ISource>();

        TargetLoadpoint? lp = null;

        source.Setup(s => s.GetCurrentLoadpointAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            (IInterfaceLogger logger) => lp);

        source.Setup(s => s.SetLoadpointAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<TargetLoadpoint>())).ReturnsAsync(
            (IInterfaceLogger logger, TargetLoadpoint loadpoint) => { lp = loadpoint; return SourceApiErrorCodes.SUCCESS; });

        services.AddSingleton(source.Object);

        var refMeter = new Mock<IRefMeter>();

        AutomaticCurrent = null;
        AutomaticPLL = null;
        AutomaticVoltage = null;
        CurrentRange = null;
        MeasurementMode = null;
        PLL = null;
        VoltageRange = null;

        refMeter.Setup(r => r.GetVoltageRangesAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            [new(500), new(250), new(100), new(50), new(2), new(1), new(0.5)]);

        refMeter.Setup(r => r.GetCurrentRangesAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            [new(500), new(250), new(100), new(50), new(2), new(1), new(0.5)]);

        refMeter.Setup(r => r.SetVoltageRangeAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<Voltage>())).Callback(
             (IInterfaceLogger logger, Voltage range) => VoltageRange = range
        );

        refMeter.Setup(r => r.SetCurrentRangeAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<Current>())).Callback(
             (IInterfaceLogger logger, Current range) => CurrentRange = range
        );

        refMeter.Setup(r => r.SetAutomaticAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback(
             (IInterfaceLogger logger, bool voltage, bool current, bool pll) =>
             {
                 AutomaticCurrent = current;
                 AutomaticPLL = pll;
                 AutomaticVoltage = voltage;
             }
        );

        refMeter.Setup(r => r.SelectPllChannelAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<PllChannel>())).Callback(
             (IInterfaceLogger logger, PllChannel pll) => PLL = pll
        );

        refMeter.Setup(r => r.SetActualMeasurementModeAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<MeasurementModes>())).Callback(
          (IInterfaceLogger logger, MeasurementModes mode) => MeasurementMode = mode
        );

        services.AddSingleton(refMeter.Object);

        var burden = new Mock<IBurden>();

        burden.Setup(b => b.GetVersionAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            (IInterfaceLogger logger) => new BurdenVersion { IsVoltageNotCurrent = IsVoltage });

        services.AddSingleton(burden.Object);

        Services = services.BuildServiceProvider();

        Hardware = Services.GetRequiredService<ICalibrationHardware>();
    }

    protected IInterfaceLogger Logger => Services.GetRequiredService<IInterfaceLogger>();

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [TestCase("200", 50, 1, 200)]
    [TestCase("200/3", 50, 1, 66.66667)]
    [TestCase("200/v3", 50, 1, 115.4700)]
    [TestCase("200", 50, 0.5, 200)]
    public async Task Can_Set_Voltage_Loadpoint_Async(string range, double frequency, double powerFactor, double expectedVaue)
    {
        IsVoltage = true;

        await Hardware.PrepareAsync(range, 1, new(frequency), false, new(new(5), new(powerFactor)));

        var lp = await Services.GetRequiredService<ISource>().GetCurrentLoadpointAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(AutomaticCurrent, Is.True);
            Assert.That(AutomaticPLL, Is.False);
            Assert.That(AutomaticVoltage, Is.True);
            Assert.That(CurrentRange, Is.Null);
            Assert.That(MeasurementMode, Is.EqualTo(MeasurementModes.MqBase));
            Assert.That(PLL, Is.EqualTo(PllChannel.U1));
            Assert.That(VoltageRange, Is.Null);

            Assert.That(lp, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(lp.Frequency.Mode, Is.EqualTo(FrequencyMode.SYNTHETIC));
            Assert.That((double)lp.Frequency.Value, Is.EqualTo(frequency));

            Assert.That(lp.Phases, Has.Count.EqualTo(3));
            Assert.That(lp.Phases[0].Current.On, Is.False);
            Assert.That(lp.Phases[0].Voltage.On, Is.True);
            Assert.That(lp.Phases[1].Current.On, Is.False);
            Assert.That(lp.Phases[1].Voltage.On, Is.False);
            Assert.That(lp.Phases[2].Current.On, Is.False);
            Assert.That(lp.Phases[2].Voltage.On, Is.False);

            Assert.That(lp.Phases[0].Current.AcComponent, Is.Not.Null);
            Assert.That(lp.Phases[0].Voltage.AcComponent, Is.Not.Null);
            Assert.That((double?)lp.Phases[0].Voltage.AcComponent?.Rms, Is.EqualTo(expectedVaue).Within(0.001));
        });
    }

    [TestCase("200", 50, 1, 200)]
    [TestCase("200/3", 50, 1, 66.66667)]
    [TestCase("200/v3", 50, 1, 115.4700)]
    [TestCase("200", 50, 0.5, 200)]
    public async Task Can_Set_Current_Loadpoint_Async(string range, double frequency, double powerFactor, double expectedVaue)
    {
        IsVoltage = false;

        await Hardware.PrepareAsync(range, 1, new(frequency), false, new(new(5), new(powerFactor)));

        var lp = await Services.GetRequiredService<ISource>().GetCurrentLoadpointAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(AutomaticCurrent, Is.True);
            Assert.That(AutomaticPLL, Is.False);
            Assert.That(AutomaticVoltage, Is.True);
            Assert.That(CurrentRange, Is.Null);
            Assert.That(MeasurementMode, Is.EqualTo(MeasurementModes.MqBase));
            Assert.That(PLL, Is.EqualTo(PllChannel.I1));
            Assert.That(VoltageRange, Is.Null);

            Assert.That(lp, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(lp.Frequency.Mode, Is.EqualTo(FrequencyMode.SYNTHETIC));
            Assert.That((double)lp.Frequency.Value, Is.EqualTo(frequency));

            Assert.That(lp.Phases, Has.Count.EqualTo(3));
            Assert.That(lp.Phases[0].Current.On, Is.True);
            Assert.That(lp.Phases[0].Voltage.On, Is.False);
            Assert.That(lp.Phases[1].Current.On, Is.False);
            Assert.That(lp.Phases[1].Voltage.On, Is.False);
            Assert.That(lp.Phases[2].Current.On, Is.False);
            Assert.That(lp.Phases[2].Voltage.On, Is.False);

            Assert.That(lp.Phases[0].Current.AcComponent, Is.Not.Null);
            Assert.That(lp.Phases[0].Current.AcComponent, Is.Not.Null);
            Assert.That((double?)lp.Phases[0].Current.AcComponent?.Rms, Is.EqualTo(expectedVaue).Within(0.001));
        });
    }

    [TestCase("2000000", 500, 0.5)]
    [TestCase("200", 250, 0.5)]
    [TestCase("100", 100, 0.5)]
    [TestCase("99", 100, 0.5)]
    [TestCase("51", 100, 0.5)]
    [TestCase("50", 50, 0.5)]
    [TestCase("1", 1, 50)]
    [TestCase("0.00001", 0.5, 500)]
    [TestCase("0", 0.5, 500)]
    public async Task Can_Calculate_Voltage_Range_Async(string range, double expectedVoltageRange, double expectedCurrentRange)
    {
        IsVoltage = true;

        await Hardware.PrepareAsync(range, 1, new(50), true, new(new(5), new(1)));

        Assert.Multiple(() =>
        {
            Assert.That(AutomaticVoltage, Is.False);
            Assert.That(AutomaticCurrent, Is.False);
            Assert.That(AutomaticPLL, Is.False);
            Assert.That(PLL, Is.EqualTo(PllChannel.U1));

            Assert.That((double?)VoltageRange, Is.EqualTo(expectedVoltageRange));
            Assert.That((double?)CurrentRange, Is.EqualTo(expectedCurrentRange));
        });
    }

    [TestCase("2000000", 500, 0.5)]
    [TestCase("200", 250, 0.5)]
    [TestCase("100", 100, 0.5)]
    [TestCase("99", 100, 0.5)]
    [TestCase("51", 100, 0.5)]
    [TestCase("50", 50, 0.5)]
    [TestCase("1", 1, 50)]
    [TestCase("0.00001", 0.5, 500)]
    [TestCase("0", 0.5, 500)]
    public async Task Can_Calculate_Current_Range_Async(string range, double expectedVoltageRange, double expectedCurrentRange)
    {
        IsVoltage = false;

        await Hardware.PrepareAsync(range, 1, new(50), true, new(new(5), new(1)));

        Assert.Multiple(() =>
        {
            Assert.That(AutomaticVoltage, Is.False);
            Assert.That(AutomaticCurrent, Is.False);
            Assert.That(AutomaticPLL, Is.False);
            Assert.That(PLL, Is.EqualTo(PllChannel.I1));

            Assert.That((double?)VoltageRange, Is.EqualTo(expectedCurrentRange));
            Assert.That((double?)CurrentRange, Is.EqualTo(expectedVoltageRange));
        });
    }

    [TestCase("200/")]
    public void Can_Not_Set_Loadpoint_Async(string range)
    {
        IsVoltage = true;

        Assert.ThrowsAsync<ArgumentException>(() => Hardware.PrepareAsync(range, 1, new(50), false, new(new(5), new(1))));
    }

    [TestCase(1, 110, 250)]
    [TestCase(1.2, 132, 250)]
    [TestCase(0.8, 88, 100)]
    public async Task Can_Set_Relative_Voltage_Loadpoint_Async(double percentage, double expectedVaue, double expectedRange)
    {
        IsVoltage = true;

        await Hardware.PrepareAsync("110", percentage, new(50), true, new(new(5), new(1)));

        var lp = await Services.GetRequiredService<ISource>().GetCurrentLoadpointAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(AutomaticVoltage, Is.False);
            Assert.That(AutomaticCurrent, Is.False);
            Assert.That(AutomaticPLL, Is.False);
            Assert.That(PLL, Is.EqualTo(PllChannel.U1));

            Assert.That((double?)VoltageRange, Is.EqualTo(expectedRange));
            Assert.That((double?)CurrentRange, Is.EqualTo(0.5));

            Assert.That(lp, Is.Not.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(lp.Phases, Has.Count.EqualTo(3));
            Assert.That((double?)lp.Phases[0].Voltage.AcComponent?.Rms, Is.EqualTo(expectedVaue).Within(0.001));
        });
    }
}