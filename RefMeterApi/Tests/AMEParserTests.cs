using System.Globalization;
using RefMeterApi.Actions.Parsers;

namespace RefMeterApiTests;

[TestFixture]
public class AMEParserTests
{
    [Test]
    public void Can_Parse_AME_Reply()
    {
        var currentCulture = Thread.CurrentThread.CurrentCulture;
        var currentUiCulture = Thread.CurrentThread.CurrentUICulture;

        Thread.CurrentThread.CurrentCulture = new CultureInfo("de");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");

        try
        {
            var parsed = MeasureValueOutputParser.Parse(File.ReadAllLines(@"TestData/ameReply.txt"));

            Assert.Multiple(() =>
            {
                Assert.That(parsed.Frequency, Is.EqualTo(50).Within(0.5));
                Assert.That(parsed.Phases[0].Voltage, Is.EqualTo(20).Within(0.5));
                Assert.That(parsed.Phases[1].Current, Is.EqualTo(0.1).Within(0.05));
                Assert.That(parsed.Phases[1].AngleVoltage, Is.EqualTo(120).Within(0.5));
                Assert.That(parsed.Phases[2].AngleCurrent, Is.EqualTo(240).Within(0.5));

                Assert.That(parsed.PhaseOrder, Is.EqualTo(123));
            });
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }

    [TestCase("-1;1")]
    [TestCase(";1")]
    [TestCase("1;")]
    [TestCase("1;1EA3", typeof(FormatException))]
    [TestCase("12.3;1")]
    [TestCase("xxxx")]
    public void Will_Fail_On_Invalid_Reply(string reply, Type? exception = null)
    {
        Assert.That(() => MeasureValueOutputParser.Parse(new[] { reply, "AMEACK" }), Throws.TypeOf(exception ?? typeof(ArgumentException)));
    }

    [Test]
    public void Will_Overwrite_Index_Value()
    {
        var data = MeasureValueOutputParser.Parse(new[] { "28;1", "28;2", "AMEACK" });

        Assert.That(data.Frequency, Is.EqualTo(2));
    }

    [Test]
    public void Can_Handle_Empty_Reply()
    {
        MeasureValueOutputParser.Parse(new[] { "AMEACK" });

        Assert.Pass();
    }

    [Test]
    public void Will_Detect_Missing_ACK()
    {
        Assert.That(() => MeasureValueOutputParser.Parse(new[] { "0;1" }), Throws.TypeOf<ArgumentException>().With.Message.Contains("AMEACK"));
    }

    [Test]
    public void Will_Detect_Misplaced_ACK()
    {
        Assert.That(() => MeasureValueOutputParser.Parse(new[] { "AMEACK", "0;1" }), Throws.TypeOf<ArgumentException>().With.Message.Contains("AMEACK"));
    }
}
