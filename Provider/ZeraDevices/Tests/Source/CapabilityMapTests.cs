using ZeraDevices.Source;

namespace ZeraDeviceTests.Source;

[TestFixture]
public class CapabilitiesMapTests
{
    [Test]
    public void Can_Read_Capabilities_Of_Supported_Models()
    {
        /* All known models. */
        Assert.That(new CapabilitiesMap().GetCapabilitiesByModel("MT786"), Is.Not.Null);
    }

    [Test]
    public void Throws_When_Reading_Capabilities_Of_Unknown_Model()
    {
        Assert.That(
            () => new CapabilitiesMap().GetCapabilitiesByModel("XXXXX"),
            Throws.TypeOf<ArgumentException>().With.Message.Contains("voltageAmplifier")
        );
    }
}
