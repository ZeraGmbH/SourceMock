using BurdenApi.Actions.Device;
using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Actions.User;
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

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddTransient<ICalibrationHardware, CalibrationHardware>();
        services.AddTransient<IInterfaceLogger, NoopInterfaceLogger>();
        services.AddTransient<ISourceHealthUtils, SourceHealthUtils>();

        var source = new Mock<ISource>();

        TargetLoadpoint? lp = null;

        source.Setup(s => s.GetCurrentLoadpointAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            (IInterfaceLogger logger) => lp);

        source.Setup(s => s.SetLoadpointAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<TargetLoadpoint>())).ReturnsAsync(
            (IInterfaceLogger logger, TargetLoadpoint loadpoint) => { lp = loadpoint; return SourceApiErrorCodes.SUCCESS; });

        source.Setup(s => s.GetCapabilitiesAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            new SourceCapabilities
            {
                Phases = { new() {
                    AcCurrent = new() { PrecisionStepSize = new(0.0001), Min = new(1), Max = new(120) },
                    AcVoltage = new() { PrecisionStepSize = new(0.001), Min = new(10), Max = new(250) }
                } }
            });

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

        refMeter.Setup(r => r.GetActualValuesUncachedAsync(It.IsAny<IInterfaceLogger>(), -1, true)).ReturnsAsync(
            new MeasuredLoadpoint
            {
                Phases = {
                    new() {
                        ActivePower = new(0),
                        ApparentPower = new(3.75),
                        PowerFactor = new(0.8),
                        ReactivePower = new(0),
                } }
            }
        );

        refMeter.Setup(r => r.SelectPllChannelAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<PllChannel>())).Callback(
            (IInterfaceLogger logger, PllChannel pll) => PLL = pll
        );

        refMeter.Setup(r => r.SetActualMeasurementModeAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<MeasurementModes>())).Callback(
          (IInterfaceLogger logger, MeasurementModes mode) => MeasurementMode = mode
        );

        refMeter.Setup(r => r.GetRefMeterStatusAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            (IInterfaceLogger logger) => new()
            {
                CurrentRange = CurrentRange ?? Current.Zero,
                VoltageRange = VoltageRange ?? Voltage.Zero,
                MeasurementMode = MeasurementMode,
                PllChannel = PLL,
            }
        );

        services.AddSingleton(refMeter.Object);

        var burden = new Mock<IBurdenMock>();

        burden.Setup(b => b.GetCalibrationAsync("IEC60", "190", "3.75;0.80", It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            (string burden, string range, string step, IInterfaceLogger logger) => new(new(0, 0), new(0, 0)));

        burden.SetupGet(b => b.HasMockedSource).Returns(true);

        var user = new Mock<ICurrentUser>();

        services.AddSingleton(user.Object);

        services.AddSingleton<IBurden>(burden.Object);

        var sourceHealthState = new Mock<SourceHealthUtils.State>();

        services.AddSingleton(sourceHealthState.Object);

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
        await Hardware.PrepareAsync(true, range, 1, new(frequency), false, new(5));

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

            Assert.That(lp.Phases[0].Current.On, Is.False);
            Assert.That(lp.Phases[0].Voltage.On, Is.True);
            Assert.That((double?)lp.Phases[0].Voltage.AcComponent?.Rms, Is.EqualTo(expectedVaue).Within(0.001));
        });
    }

    [TestCase("200", 1, 1)]
    [TestCase("200/3", 1, 1)]
    [TestCase("200/v3", 1, 1)]
    [TestCase("200/3", 0.1, 0.15)]
    [TestCase("200", 1.5, 1.25)]
    public async Task Can_Adapt_Voltage_Percentage_Async(string range, double percentage, double expected)
    {
        for (var adjust = 2; adjust-- > 0;)
            Assert.That(
                (await Hardware.PrepareAsync(true, range, percentage, new(50), false, new(5), adjust != 1)).Factor,
                Is.EqualTo(adjust == 1 ? expected : percentage).Within(0.001)
            );
    }

    [TestCase("200", 1, 0.6)]
    [TestCase("200/3", 1, 1)]
    [TestCase("200/v3", 1, 1)]
    [TestCase("5/3", 0.1, 0.6)]
    public async Task Can_Adapt_Current_Percentage_Async(string range, double percentage, double expected)
    {
        for (var adjust = 2; adjust-- > 0;)
            Assert.That(
                (await Hardware.PrepareAsync(false, range, percentage, new(50), false, new(5), adjust != 1)).Factor,
                Is.EqualTo(adjust == 1 ? expected : percentage).Within(0.001)
            );
    }

    [TestCase("200", 50, 1, 20)]
    [TestCase("200/3", 50, 1, 6.666667)]
    [TestCase("200/v3", 50, 1, 11.54700)]
    [TestCase("200", 50, 0.5, 20)]
    public async Task Can_Set_Current_Loadpoint_Async(string range, double frequency, double powerFactor, double expectedVaue)
    {
        await Hardware.PrepareAsync(false, range, 0.1, new(frequency), false, new(5));

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
        await Hardware.PrepareAsync(true, range, 1, new(50), true, new(5));

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

    [TestCase("2000000", 0.5, 500)]
    [TestCase("200", 0.5, 50)]
    [TestCase("100", 0.5, 50)]
    [TestCase("99", 0.5, 50)]
    [TestCase("51", 0.5, 50)]
    [TestCase("50", 0.5, 50)]
    [TestCase("1", 0.5, 0.5)]
    [TestCase("0.00001", 500, 0.5)]
    public async Task Can_Calculate_Current_Range_Async(string range, double expectedVoltageRange, double expectedCurrentRange)
    {
        await Hardware.PrepareAsync(false, range, 0.1, new(50), true, new(5));

        Assert.Multiple(() =>
        {
            Assert.That(AutomaticVoltage, Is.False);
            Assert.That(AutomaticCurrent, Is.False);
            Assert.That(AutomaticPLL, Is.False);
            Assert.That(PLL, Is.EqualTo(PllChannel.I1));

            Assert.That((double?)VoltageRange, Is.EqualTo(expectedVoltageRange));
            Assert.That((double?)CurrentRange, Is.EqualTo(expectedCurrentRange));
        });
    }

    [TestCase("200/")]
    public void Can_Not_Set_Loadpoint_Async(string range)
    {
        Assert.ThrowsAsync<ArgumentException>(() => Hardware.PrepareAsync(true, range, 1, new(50), false, new(5)));
    }

    [TestCase(1, 110, 250)]
    [TestCase(1.2, 132, 250)]
    [TestCase(0.8, 88, 100)]
    public async Task Can_Set_Relative_Voltage_Loadpoint_Async(double percentage, double expectedVaue, double expectedRange)
    {
        await Hardware.PrepareAsync(true, "110", percentage, new(50), true, new(5));

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

    [Test]
    public async Task Can_Measure_With_Auto_Range_Async()
    {
        await Hardware.PrepareAsync(true, "190", 1, new(60), false, new(3.75));

        Assert.Multiple(() =>
        {
            Assert.That(VoltageRange, Is.Null);
            Assert.That(CurrentRange, Is.Null);
        });

        var values = await Hardware.MeasureAsync(new(), true);

        Assert.That((double)values.PowerFactor, Is.EqualTo(0.8));
    }

    [Test]
    public async Task Can_Measure_With_Detect_Range_Async()
    {
        await Hardware.PrepareAsync(true, "190", 1, new(60), true, new(3.75));

        Assert.Multiple(() =>
        {
            Assert.That((double?)VoltageRange, Is.EqualTo(250));
            Assert.That((double?)CurrentRange, Is.EqualTo(0.5));
        });

        CurrentRange = new(250);
        VoltageRange = new(50);

        var values = await Hardware.MeasureAsync(new(), true);

        Assert.Multiple(() =>
        {
            Assert.That((double?)VoltageRange, Is.EqualTo(250));
            Assert.That((double?)CurrentRange, Is.EqualTo(0.5));
        });

        Assert.That((double)values.PowerFactor, Is.EqualTo(0.8));
    }

    [Test]
    public async Task Problem_With_Current_Burden_Async()
    {
        var source = new Mock<ISource>();

        var capabilities = new SourceCapabilities
        {
            Phases = [
                    new() {
                        AcVoltage = new() { Min = new(30), Max = new(480), PrecisionStepSize = new(0.001) },
                        AcCurrent = new() { Min = new(0.0005), Max = new(160), PrecisionStepSize = new(0.0001), },
                        DcVoltage = null,
                        DcCurrent = null
                    },
                    new() {
                        AcVoltage = new() { Min = new(30), Max = new(480), PrecisionStepSize = new(0.001) },
                        AcCurrent = new() { Min = new(0.0005), Max = new(160), PrecisionStepSize = new(0.0001), },
                        DcVoltage = null,
                        DcCurrent = null
                    },
                    new() {
                        AcVoltage = new() { Min = new(30), Max = new(480), PrecisionStepSize = new(0.001) },
                        AcCurrent = new() { Min = new(0.0005), Max = new(160), PrecisionStepSize = new(0.0001), },
                        DcVoltage = null,
                        DcCurrent = null
                    },
                ],
            FrequencyRanges = [
                    new() { Mode = FrequencyMode.SYNTHETIC, Min = new(40), Max = new(70), PrecisionStepSize = new(0.01) }
                ]
        };

        source
            .Setup(s => s.GetCapabilitiesAsync(It.IsAny<IInterfaceLogger>()))
            .ReturnsAsync(capabilities);

        source
            .Setup(s => s.SetLoadpointAsync(It.IsAny<IInterfaceLogger>(), It.IsAny<TargetLoadpoint>()))
            .ReturnsAsync((IInterfaceLogger logger, TargetLoadpoint loadpoint) =>
            {
                var validator = new SourceCapabilityValidator();

                Assert.That(validator.IsValid(loadpoint, capabilities), Is.EqualTo(SourceApiErrorCodes.SUCCESS));

                return SourceApiErrorCodes.SUCCESS;
            });

        var refMeter = new Mock<IRefMeter>();

        var burden = new Mock<IBurden>();

        var user = new Mock<ICurrentUser>();

        var hardware = new CalibrationHardware(source.Object, new SourceHealthUtils(source.Object, user.Object, new()), refMeter.Object, burden.Object, new NoopInterfaceLogger());

        var factor = await hardware.PrepareAsync(
            false,
            "5",
            0.1,
            new(50),
            false,
            new(1)
        );

        Assert.That(factor.Factor, Is.EqualTo(0.1));
    }
}