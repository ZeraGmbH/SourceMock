using RefMeterApi.Actions.Device;
using Moq;
using SourceApi.Actions.Source;
using RefMeterApi.Models;
using SourceApi.Model;

namespace RefMeterApiTests;

public class RefMeterMockTest
{
    [Test]
    public void Produces_Actual_Values_If_No_Loadpoint_Switched_On()
    {
        var sourceMock = new Mock<ISource>();

        RefMeterMock refMeterMock = new(sourceMock.Object);

        MeasureOutput measureOutput = refMeterMock.GetActualValues().Result;

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

        var sourceMock = new Mock<ISource>();
        sourceMock.Setup(s => s.GetCurrentLoadpoint()).Returns(
            new Loadpoint()
            {
                Frequency = new() { Value = frequencyValue },
                Phases = new List<PhaseLoadpoint>(){
                    new PhaseLoadpoint(){
                        Current = new(){Rms=current, Angle=currentAngle},
                        Voltage = new(){Rms=voltage, Angle=voltageAngle}
                    }
                }
            });

        RefMeterMock refMeterMock = new(sourceMock.Object);
        MeasureOutput measureOutput = refMeterMock.GetActualValues().Result;

        Assert.That(measureOutput.Frequency, Is.InRange(GetMinValue(frequencyValue, 0.0002), GetMaxValue(frequencyValue, 0.0002)));
        Assert.That(measureOutput.Phases[0].Current, Is.InRange(GetMinValue(current, 0.0001), GetMaxValue(current, 0.0001)));
        Assert.That(measureOutput.Phases[0].AngleCurrent, Is.InRange(GetAbsoluteMinValue(currentAngle, 0.1), GetAbsoluteMaxValue(current, 0.1)));
        Assert.That(measureOutput.Phases[0].Voltage, Is.InRange(GetMinValue(voltage, 0.1), GetMaxValue(voltage, 0.0005)));
        Assert.That(measureOutput.Phases[0].AngleVoltage, Is.InRange(GetAbsoluteMinValue(voltageAngle, 0.1), GetAbsoluteMaxValue(voltageAngle, 0.1)));
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
}
