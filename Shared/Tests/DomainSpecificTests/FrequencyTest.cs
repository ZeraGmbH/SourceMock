using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class FrequencyTest
{
    [TestCase(1d, 10d)]
    [TestCase(12d, 60d)]
    public void Can_Calculate_Frequency_Max_Right(double min, double max)
    {
        var minFrequency = new Frequency(min);
        var maxFrequency = new Frequency(max);

        var actualMax = minFrequency.Largest(maxFrequency);

        Assert.That((double)actualMax, Is.EqualTo(max).Within(0.001));
    }

    [TestCase(100d, 10d)]
    [TestCase(102d, 60d)]
    public void Can_Calculate_Frequency_Max_Left(double max, double min)
    {
        var minFrequency = new Frequency(min);
        var maxFrequency = new Frequency(max);

        var actualMax = minFrequency.Largest(maxFrequency);

        Assert.That((double)actualMax, Is.EqualTo(max).Within(0.001));
    }

    [TestCase(100d, 10d)]
    [TestCase(102d, 60d)]
    public void Can_Calculate_Frequency_Min_Right(double max, double min)
    {
        var minFrequency = new Frequency(min);
        var maxFrequency = new Frequency(max);

        var actualMax = minFrequency.Smallest(maxFrequency);

        Assert.That((double)actualMax, Is.EqualTo(min).Within(0.001));
    }

    [TestCase(1d, 10d)]
    [TestCase(12d, 60d)]
    public void Can_Calculate_Frequency_Min_Left(double min, double max)
    {
        var minFrequency = new Frequency(min);
        var maxFrequency = new Frequency(max);

        var actualMax = minFrequency.Smallest(maxFrequency);

        Assert.That((double)actualMax, Is.EqualTo(min).Within(0.001));
    }
}