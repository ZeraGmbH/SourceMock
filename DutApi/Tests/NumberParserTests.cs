using System.Globalization;
using DutApi.Actions;

namespace DutApiTests;

[TestFixture]
public class NumberParserTests
{
    [TestCase("3.6e+8", 360000000)]
    [TestCase("-3000E-2", -30)]
    [TestCase(".E12", null)]
    [TestCase("+1.E1", 10)]
    [TestCase("+.1E1", 1)]
    [TestCase("29091963", 29091963)]
    [TestCase("00029091963", 29091963)]
    public void Can_Parse_Numbers(string rawValue, double? expected)
    {
        var match = ScpiConnection.ParseNumber.Match(rawValue);

        Assert.That(match?.Success, Is.EqualTo(expected != null));

        if (expected != null)
            Assert.That(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), Is.EqualTo(expected));
    }
}
