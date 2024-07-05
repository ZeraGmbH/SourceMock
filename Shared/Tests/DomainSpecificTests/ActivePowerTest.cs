using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class ActivePowerTest
{
    [TestCase(1000d, 100d, 27.7777d)]
    [TestCase(2d, 500d, 0.27777d)]
    public void Can_Calculate_Active_Energy(double powerValue, double timeValue, double energyValue)
    {
        var activePower = new ActivePower(powerValue);
        var timeInSeconds = new Time(timeValue);

        var expectedEnergy = new ActiveEnergy(energyValue);

        var actualImpulses = activePower * timeInSeconds;

        Assert.That((double)expectedEnergy, Is.EqualTo((double)actualImpulses).Within(0.001));
    }
}