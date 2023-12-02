using SourceApi.Actions.SerialPort;

namespace SourceApi.Tests.Actions.SerialPort;

[TestFixture]
public class CapabilitiesMapTests
{
    [Test]
    public void Can_Read_Capabilities_Of_Supported_Models()
    {
        /* The special mock model */
        Assert.That(CapabilitiesMap.GetCapabilitiesByModel("MT793"), Is.Not.Null);

        /* All known models. */
        Assert.That(CapabilitiesMap.GetCapabilitiesByModel("MT786"), Is.Not.Null);
    }

    [Test]
    public void Throws_When_Reading_Capabilities_Of_Unknown_Model()
    {
        Assert.That(
            () => CapabilitiesMap.GetCapabilitiesByModel("XXXXX"),
            Throws.TypeOf<ArgumentException>().With.Message.Contains("XXXXX")
        );
    }
}
