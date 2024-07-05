using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class AngleTest
{
    [TestCase(-100d, 260d)]
    [TestCase(1324874d, 74d)]
    [TestCase(7d, 7d)]
    [TestCase(-7d, 353d)]
    [TestCase(360d, 0d)]
    [TestCase(0d, 0d)]
    [TestCase(-0d, 0d)]
    public void Can_Normalize_Angles(double angleInput, double expectedAngleValue)
    {
        var expectedAngle = new Angle(expectedAngleValue);
        var actualAngle = new Angle(angleInput).Normalize();

        Assert.That((double)expectedAngle, Is.EqualTo((double)actualAngle));
    }

    [TestCase(0d, 0d)]
    [TestCase(30d, 0.5d)]
    [TestCase(90d, 1d)]
    [TestCase(150d, 0.5d)]
    public void Can_Calculate_Sin(double angleInput, double expectedSin)
    {
        var actualSin = new Angle(angleInput).Sin();

        Assert.That((double)expectedSin, Is.EqualTo((double)actualSin).Within(0.001));
    }

    [TestCase(0d, 1d)]
    [TestCase(60d, 0.5d)]
    [TestCase(90d, 0d)]
    [TestCase(300d, 0.5d)]
    public void Can_Calculate_Cos(double angleInput, double expectedCos)
    {
        var actualSin = new Angle(angleInput).Cos();

        Assert.That((double)expectedCos, Is.EqualTo((double)actualSin).Within(0.001));
    }
}