using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class ActiveEnergyTest
{
    [TestCase(100d, 10000d, 1000d)]
    [TestCase(239.45, 111, 26.57895)]
    public void Can_Calculate_Impulses(double energyValue, double meterConstantValue, double impulsesValue)
    {
        var activeEnergy = new ActiveEnergy(energyValue);
        var meterConstant = new MeterConstant(meterConstantValue);

        var expectedImpulses = new Impulses(impulsesValue);

        var actualImpulses = activeEnergy * meterConstant;
        Assert.That((double)expectedImpulses, Is.EqualTo((double)actualImpulses));
    }
}