using System.Diagnostics.Metrics;
using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class ImpulsesTest
{
    [TestCase(123d, 10d, 12300d)]
    [TestCase(12.1d, 60d, 201.66d)]
    public void Can_Calculate_Active_Energy(double impulsesValue, double meterConstantValue, double activeEnergyValue)
    {
        var impulses = new Impulses(impulsesValue);
        var meterConstant = new MeterConstant(meterConstantValue);

        var expectedActiveEnergy = new ActiveEnergy(activeEnergyValue);

        var actualActiveEnergy = impulses / meterConstant;

        Assert.That((double)actualActiveEnergy, Is.EqualTo((double)expectedActiveEnergy).Within(0.01));
    }

}