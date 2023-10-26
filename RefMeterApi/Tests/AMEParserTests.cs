using RefMeterApi.Actions.Parsers;

namespace RefMeterApiTests;

[TestFixture]
public class AMEParserTests
{
    [Test]
    public void Can_Parse_AME_Reply()
    {
        var parsed = MeasureValueOutputParser.Parse(File.ReadAllLines(@"TestData/ameReply.txt"));

        Assert.That(parsed.Frequency, Is.EqualTo(50).Within(0.5));
        Assert.That(parsed.Phases[0].Voltage, Is.EqualTo(20).Within(0.5));
        Assert.That(parsed.Phases[1].Current, Is.EqualTo(0.1).Within(0.05));
        Assert.That(parsed.Phases[1].AngleVoltage, Is.EqualTo(120).Within(0.5));
        Assert.That(parsed.Phases[2].AngleCurrent, Is.EqualTo(240).Within(0.5));

        Assert.That(parsed.PhaseOrder, Is.EqualTo(123));
    }
}
