using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class CurrentTest
{
    [TestCase(100d, 20d, 2000d)]
    [TestCase(12d, 60d, 720d)]
    public void Can_Calculate_Apparent_Power(double voltageValue, double currentValue, double expectedApparentPower)
    {
        var voltage = new Voltage(voltageValue);
        var current = new Current(currentValue);

        var actualApparentPower = current * voltage;

        Assert.That((double)expectedApparentPower, Is.EqualTo((double)actualApparentPower).Within(0.001));
    }


    [TestCase(1d, 10d)]
    [TestCase(12d, 60d)]
    public void Can_Calculate_Current_Max_Right(double min, double max)
    {
        var minCurrent = new Current(min);
        var maxCurrent = new Current(max);

        var actualMax = minCurrent.Largest(maxCurrent);

        Assert.That((double)actualMax, Is.EqualTo(max).Within(0.001));
    }

    [TestCase(100d, 10d)]
    [TestCase(102d, 60d)]
    public void Can_Calculate_Current_Max_Left(double max, double min)
    {
        var minCurrent = new Current(min);
        var maxCurrent = new Current(max);

        var actualMax = minCurrent.Largest(maxCurrent);

        Assert.That((double)actualMax, Is.EqualTo(max).Within(0.001));
    }

    [TestCase(100d, 10d)]
    [TestCase(102d, 60d)]
    public void Can_Calculate_Current_Min_Right(double max, double min)
    {
        var minCurrent = new Current(min);
        var maxCurrent = new Current(max);

        var actualMax = minCurrent.Smallest(maxCurrent);

        Assert.That((double)actualMax, Is.EqualTo(min).Within(0.001));
    }

    [TestCase(1d, 10d)]
    [TestCase(12d, 60d)]
    public void Can_Calculate_Current_Min_Left(double min, double max)
    {
        var minCurrent = new Current(min);
        var maxCurrent = new Current(max);

        var actualMax = minCurrent.Smallest(maxCurrent);

        Assert.That((double)actualMax, Is.EqualTo(min).Within(0.001));
    }
}