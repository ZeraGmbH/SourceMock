using System.Diagnostics.Metrics;
using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class MeterConstantTest
{
    [TestCase(123d, 1021d, 125d)]
    [TestCase(12.1d, 6044444d, 73137d)]
    public void Can_Calculate_Active_Energy(double meterConstantValue, double activeEnergyValue, double impulsesValue)
    {
        var meterConstant = new MeterConstant(meterConstantValue);
        var activeEnergy = new ActiveEnergy(activeEnergyValue);

        var expectedImpulses = new Impulses(impulsesValue);

        var actualImpulses = meterConstant * activeEnergy;

        Assert.That((double)actualImpulses, Is.EqualTo((double)expectedImpulses).Within(0.01));
    }

}