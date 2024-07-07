using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.MeterConstantCalculator;
using RefMeterApi.Models;

namespace RefMeterApiTests;

[TestFixture]
public class MeterConstantCalculatorTests
{
    [TestCase(ReferenceMeters.EPZ303x1, 60000d, MeasurementModes.FourWireActivePower, 240d, 5d, 60000000d)]
    [TestCase(ReferenceMeters.COM3003x1x2, 60000d, MeasurementModes.FourWireActivePower, 240d, 5d, 60000000d)]
    [TestCase(ReferenceMeters.EPZ303x10, 60000d, MeasurementModes.FourWireActivePower, 120d, 2d, 300000000d)]
    [TestCase(ReferenceMeters.COM3003, 30000d, MeasurementModes.ThreeWireActivePower, 120d, 2d, 86602540.378d)]
    [TestCase(ReferenceMeters.EPZ303x8, 80000d, MeasurementModes.TwoWireActivePower, 120d, 2d, 1200000000d)]
    public void Test_Calculations(ReferenceMeters meter, double frequency, MeasurementModes mode, double voltage, double current, double expected)
    {
        var cut = new MeterConstantCalculator(new NullLogger<MeterConstantCalculator>());

        Assert.That((double)cut.GetMeterConstant(meter, new(frequency), mode, new(voltage), new(current)), Is.EqualTo(expected).Within(0.001));
    }
}