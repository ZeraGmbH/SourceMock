using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class PowerFactorTest
{
    [TestCase(60d, 0.5)]
    [TestCase(0d, 1d)]
    [TestCase(90d, 0d)]
    public void Can_Instantiate_Power_Factor(double angle, double expectedPowerFactor)
    {
        var actualPowerFactor = new PowerFactor(new Angle(angle));

        Assert.That((double)actualPowerFactor, Is.EqualTo(expectedPowerFactor).Within(0.01));
    }

    [TestCase(60d, 0.5d)]
    public void Can_Cast_Power_Factor_To_Angle(double expectedAngle, double powerFactorValue)
    {
        var expectedPowerFactor = new PowerFactor(new Angle(expectedAngle));

        Assert.That((double)expectedPowerFactor, Is.EqualTo(powerFactorValue).Within(0.01));

        var actualAngle = (Angle)expectedPowerFactor;

        Assert.That((double)actualAngle, Is.EqualTo(expectedAngle).Within(0.01));
    }
}