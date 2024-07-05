using SharedLibrary.DomainSpecific;

namespace SharedLibrary.Tests.DomainSpecificTests;

public class VoltageTest
{

    [TestCase(1d, 10d)]
    [TestCase(12d, 60d)]
    public void Can_Calculate_Voltage_Max_Right(double min, double max)
    {
        var minVoltage = new Voltage(min);
        var maxVoltage = new Voltage(max);

        var actualMax = Voltage.Max(minVoltage, maxVoltage);

        Assert.That((double)actualMax, Is.EqualTo(max).Within(0.001));
    }

    [TestCase(100d, 10d)]
    [TestCase(102d, 60d)]
    public void Can_Calculate_Voltage_Max_Left(double max, double min)
    {
        var minVoltage = new Voltage(min);
        var maxVoltage = new Voltage(max);

        var actualMax = Voltage.Max(minVoltage, maxVoltage);

        Assert.That((double)actualMax, Is.EqualTo(max).Within(0.001));
    }

    [TestCase(100d, 10d)]
    [TestCase(102d, 60d)]
    public void Can_Calculate_Voltage_Min_Right(double max, double min)
    {
        var minVoltage = new Voltage(min);
        var maxVoltage = new Voltage(max);

        var actualMax = Voltage.Min(minVoltage, maxVoltage);

        Assert.That((double)actualMax, Is.EqualTo(min).Within(0.001));
    }

    [TestCase(1d, 10d)]
    [TestCase(12d, 60d)]
    public void Can_Calculate_Voltage_Min_Left(double min, double max)
    {
        var minVoltage = new Voltage(min);
        var maxVoltage = new Voltage(max);

        var actualMax = Voltage.Min(minVoltage, maxVoltage);

        Assert.That((double)actualMax, Is.EqualTo(min).Within(0.001));
    }
}