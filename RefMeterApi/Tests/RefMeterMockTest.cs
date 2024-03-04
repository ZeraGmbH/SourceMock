using RefMeterApi.Actions.Device;
using Moq;
using SourceApi.Actions.Source;
using RefMeterApi.Models;
using SourceApi.Model;
using Microsoft.Extensions.DependencyInjection;

namespace RefMeterApiTests;

public class RefMeterMockTest
{
    private ServiceProvider Services;

    private Mock<ISource> SourceMock;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        SourceMock = new Mock<ISource>();

        services.AddSingleton(SourceMock.Object);

        Services = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [Test]
    public void Produces_Actual_Values_If_No_Loadpoint_Switched_On()
    {
        RefMeterMock refMeterMock = new(Services);

        MeasuredLoadpoint measureOutput = refMeterMock.GetActualValues().Result;

        Assert.That(measureOutput.Frequency, Is.EqualTo(0));
    }

    [Test]
    public void Produces_Actual_Values_If_Loadpoint_Switched_On()
    {
        double frequencyValue = 99;
        double current = 22222222;
        double voltage = 33333333;
        double currentAngle = 7;
        double voltageAngle = 5;

        SourceMock.Setup(s => s.GetCurrentLoadpoint()).Returns(
            new TargetLoadpoint()
            {
                Frequency = new() { Value = frequencyValue },
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new(){AcComponent = new() {Rms=current, Angle=currentAngle}, On=true},
                        Voltage = new(){AcComponent = new() {Rms=voltage, Angle=voltageAngle}, On=true}
                    }
                }
            });

        SourceMock.Setup(s => s.GetActiveLoadpointInfo()).Returns(new LoadpointInfo { IsActive = true });
        SourceMock.Setup(s => s.CurrentSwitchedOffForDosage()).ReturnsAsync(false);

        RefMeterMock refMeterMock = new(Services);
        MeasuredLoadpoint measureOutput = refMeterMock.GetActualValues().Result;

        Assert.That(measureOutput.Frequency, Is.InRange(GetMinValue(frequencyValue, 0.0002), GetMaxValue(frequencyValue, 0.0002)));
        Assert.That(measureOutput.Phases[0].Current.AcComponent.Rms, Is.InRange(GetMinValue(current, 0.0001), GetMaxValue(current, 0.0001)));
        Assert.That(measureOutput.Phases[0].Current.AcComponent.Angle, Is.InRange(GetAbsoluteMinValue(currentAngle, 0.1), GetAbsoluteMaxValue(current, 0.1)));
        Assert.That(measureOutput.Phases[0].Voltage.AcComponent.Rms, Is.InRange(GetMinValue(voltage, 0.0005), GetMaxValue(voltage, 0.0005)));
        Assert.That(measureOutput.Phases[0].Voltage.AcComponent.Angle, Is.InRange(GetAbsoluteMinValue(voltageAngle, 0.1), GetAbsoluteMaxValue(voltageAngle, 0.1)));
    }

    [Test]
    public void Transfers_Data_Correctly_To_Measure_Output()
    {
        // Arrange
        TargetLoadpoint lp = RefMeterMockTestData.Loadpoint_OnlyActivePower;

        // Act
        var mo = RefMeterMock.CalcMeasureOutput(lp);

        // Assert
        Assert.That(mo.Frequency, Is.EqualTo(50).Within(double.Epsilon));

        Assert.That(mo.Phases.Count, Is.EqualTo(3));

        Assert.That(mo.Phases[0].Voltage.AcComponent.Rms, Is.EqualTo(230).Within(double.Epsilon));
        Assert.That(mo.Phases[0].Voltage.AcComponent.Angle, Is.EqualTo(0).Within(double.Epsilon));
        Assert.That(mo.Phases[0].Current.AcComponent.Rms, Is.EqualTo(100).Within(double.Epsilon));
        Assert.That(mo.Phases[0].Current.AcComponent.Angle, Is.EqualTo(0).Within(double.Epsilon));

        Assert.That(mo.Phases[1].Voltage.AcComponent.Rms, Is.EqualTo(235).Within(double.Epsilon));
        Assert.That(mo.Phases[1].Voltage.AcComponent.Angle, Is.EqualTo(120).Within(double.Epsilon));
        Assert.That(mo.Phases[1].Current.AcComponent.Rms, Is.EqualTo(80).Within(double.Epsilon));
        Assert.That(mo.Phases[1].Current.AcComponent.Angle, Is.EqualTo(120).Within(double.Epsilon));

        Assert.That(mo.Phases[2].Voltage.AcComponent.Rms, Is.EqualTo(240).Within(double.Epsilon));
        Assert.That(mo.Phases[2].Voltage.AcComponent.Angle, Is.EqualTo(240).Within(double.Epsilon));
        Assert.That(mo.Phases[2].Current.AcComponent.Rms, Is.EqualTo(60).Within(double.Epsilon));
        Assert.That(mo.Phases[2].Current.AcComponent.Angle, Is.EqualTo(240).Within(double.Epsilon));
    }

    [Test]
    public void Calculates_Measure_Output_Correctly_Only_Active_Power()
    {
        // Arrange
        TargetLoadpoint lp = RefMeterMockTestData.Loadpoint_OnlyActivePower;

        // Act
        var mo = RefMeterMock.CalcMeasureOutput(lp);

        // Assert
        Assert.That(mo.Phases[0].ActivePower, Is.EqualTo(23000).Within(10e-10));
        Assert.That(mo.Phases[0].ReactivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.Phases[0].ApparentPower, Is.EqualTo(23000).Within(10e-10));

        Assert.That(mo.Phases[1].ActivePower, Is.EqualTo(18800).Within(10e-10));
        Assert.That(mo.Phases[1].ReactivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.Phases[1].ApparentPower, Is.EqualTo(18800).Within(10e-10));

        Assert.That(mo.Phases[2].ActivePower, Is.EqualTo(14400).Within(10e-10));
        Assert.That(mo.Phases[2].ReactivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.Phases[2].ApparentPower, Is.EqualTo(14400).Within(10e-10));

        Assert.That(mo.ActivePower, Is.EqualTo(56200).Within(10e-10));
        Assert.That(mo.ReactivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.ApparentPower, Is.EqualTo(56200).Within(10e-10));
    }

    [Test]
    public void Calculates_Measure_Output_Correctly_Only_Rective_Power()
    {
        // Arrange
        TargetLoadpoint lp = RefMeterMockTestData.Loadpoint_OnlyReactivePower;

        // Act
        var mo = RefMeterMock.CalcMeasureOutput(lp);

        // Assert
        Assert.That(mo.Phases[0].ActivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.Phases[0].ReactivePower, Is.EqualTo(23000).Within(10e-10));
        Assert.That(mo.Phases[0].ApparentPower, Is.EqualTo(23000).Within(10e-10));

        Assert.That(mo.Phases[1].ActivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.Phases[1].ReactivePower, Is.EqualTo(18800).Within(10e-10));
        Assert.That(mo.Phases[1].ApparentPower, Is.EqualTo(18800).Within(10e-10));

        Assert.That(mo.Phases[2].ActivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.Phases[2].ReactivePower, Is.EqualTo(14400).Within(10e-10));
        Assert.That(mo.Phases[2].ApparentPower, Is.EqualTo(14400).Within(10e-10));

        Assert.That(mo.ActivePower, Is.EqualTo(0).Within(10e-10));
        Assert.That(mo.ReactivePower, Is.EqualTo(56200).Within(10e-10));
        Assert.That(mo.ApparentPower, Is.EqualTo(56200).Within(10e-10));
    }

    [Test]
    public void Calculates_Measure_Output_Correctly_Active_Rective_Mixed_Power()
    {
        // Arrange
        TargetLoadpoint lp = RefMeterMockTestData.Loadpoint_CosPhi0_5;

        // Act
        var mo = RefMeterMock.CalcMeasureOutput(lp);

        // Assert
        Assert.That(mo.Phases[0].ActivePower, Is.EqualTo(11500).Within(10e-10));
        Assert.That(mo.Phases[0].ReactivePower, Is.EqualTo(19918.58).Within(0.01));
        Assert.That(mo.Phases[0].ApparentPower, Is.EqualTo(23000).Within(10e-10));

        Assert.That(mo.Phases[1].ActivePower, Is.EqualTo(9400).Within(10e-10));
        Assert.That(mo.Phases[1].ReactivePower, Is.EqualTo(16281.27).Within(0.01));
        Assert.That(mo.Phases[1].ApparentPower, Is.EqualTo(18800).Within(10e-10));

        Assert.That(mo.Phases[2].ActivePower, Is.EqualTo(7200).Within(10e-10));
        Assert.That(mo.Phases[2].ReactivePower, Is.EqualTo(12470.765814496).Within(10e-10));
        Assert.That(mo.Phases[2].ApparentPower, Is.EqualTo(14400).Within(10e-10));

        Assert.That(mo.ActivePower, Is.EqualTo(28100).Within(10e-10));
        Assert.That(mo.ReactivePower, Is.EqualTo(48670.62).Within(0.01));
        Assert.That(mo.ApparentPower, Is.EqualTo(56200).Within(10e-10));
    }

    private double GetMinValue(double value, double deviation)
    {
        return value - value * deviation;
    }

    private double GetMaxValue(double value, double deviation)
    {
        return value + value * deviation;
    }

    private double GetAbsoluteMinValue(double value, double deviation)
    {
        return value - deviation;
    }

    private double GetAbsoluteMaxValue(double value, double deviation)
    {
        return value + deviation;
    }

    [TestCase(0d, 120d, 240d, "132")]
    [TestCase(240d, 0d, 120d, "132")]
    [TestCase(120d, 240d, 0d, "132")]
    [TestCase(0d, 240d, 120d, "123")]
    [TestCase(120d, 0d, 240d, "123")]
    [TestCase(240d, 120d, 0d, "123")]
    public void Will_Calculate_Phase_Order(double angle0, double angle1, double angle2, string phaseOrder)
    {
        // Arrange
        var lp = new TargetLoadpoint()
        {
            Frequency = new() { Value = 50 },
            Phases = [
                     new () {
                        Current = new(){AcComponent = new() {Rms=1, Angle=angle0}},
                        Voltage = new(){AcComponent = new() {Rms=220, Angle=angle0}}
                    },
                    new () {
                        Current = new(){AcComponent = new() {Rms=1, Angle=angle1}},
                        Voltage = new(){AcComponent = new() {Rms=220, Angle=angle1}}
                    },
                    new () {
                        Current = new(){AcComponent = new() {Rms=1, Angle=angle2}},
                        Voltage = new(){AcComponent = new() {Rms=220, Angle=angle2}}
                    }
                 ]
        };

        // Act
        var mo = RefMeterMock.CalcMeasureOutput(lp);

        // Assert
        Assert.That(mo.PhaseOrder, Is.EqualTo(phaseOrder));
    }
}
