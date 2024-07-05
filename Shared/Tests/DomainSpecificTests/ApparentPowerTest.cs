using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class ApparentPowerTest
{
    [TestCase(1110d, 0d, 1110d)]
    [TestCase(1110d, 60d, 555.0d)]
    [TestCase(1110d, 90d, 0d)]
    public void Can_Calculate_Active_Energy(double apparentPowerValue, double phaseAngleValue, double expectedActiveEnergy)
    {
        var apparentPower = new ApparentPower(apparentPowerValue);
        var phaseAngle = new Angle(phaseAngleValue);

        var actualActiveEnergy = apparentPower.GetActivePower(phaseAngle);

        Assert.That((double)expectedActiveEnergy, Is.EqualTo((double)actualActiveEnergy).Within(0.001));
    }

    [TestCase(1110d, 0d, 0d)]
    [TestCase(1110d, 60d, 961.288d)]
    [TestCase(1110d, 30d, 555.0d)]
    [TestCase(1110d, 90d, 1110d)]
    public void Can_Calculate_Rective_Energy(double apparentPowerValue, double phaseAngleValue, double expectedActiveEnergy)
    {
        var apparentPower = new ApparentPower(apparentPowerValue);
        var phaseAngle = new Angle(phaseAngleValue);

        var actualActiveEnergy = apparentPower.GetReactivePower(phaseAngle);

        Assert.That((double)expectedActiveEnergy, Is.EqualTo((double)actualActiveEnergy).Within(0.001));
    }
}