using System.Text.Json;
using SharedLibrary;
using SharedLibrary.DomainSpecific;

namespace SharedLibraryTests;

[TestFixture]
public class DomainSpecificNumberTests
{
    public class Combined
    {
        public ActiveEnergie Energy1 { get; set; } = null!;

        public ActiveEnergie? Energy2 { get; set; }
    }

    [TestCase(33.12, "33.12")]
    public void Domain_Specific_Number_Will_Serialize_To_Number(double energy, string expected)
    {
        var num = new ActiveEnergie(energy);
        var json = JsonSerializer.Serialize(num, LibUtils.JsonSettings);

        Assert.That(json, Is.EqualTo(expected));

        var back = JsonSerializer.Deserialize<ActiveEnergie>(json, LibUtils.JsonSettings);

        Assert.That(back, Is.TypeOf<ActiveEnergie>());
        Assert.That((double)back, Is.EqualTo(energy));
    }

    [Test]
    public void Can_Serialize_Model_With_Domain_Specific_Number()
    {
        var comp = new Combined { Energy1 = new ActiveEnergie(-12.12) };
        var json = JsonSerializer.Serialize(comp, LibUtils.JsonSettings);

        Assert.That(json, Is.EqualTo(@"{""energy1"":-12.12,""energy2"":null}"));

        var back = JsonSerializer.Deserialize<Combined>(json, LibUtils.JsonSettings);

        Assert.That(back, Is.TypeOf<Combined>());
        Assert.That((double)back.Energy1, Is.EqualTo(-12.12));
        Assert.That(back.Energy2, Is.Null);
    }

    [Test]
    public void Can_Deserialize_Null()
    {
        var back = JsonSerializer.Deserialize<ActiveEnergie>("null", LibUtils.JsonSettings);

        Assert.That(back, Is.Null);
    }
}