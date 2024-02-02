using SharedLibrary;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedLibraryTests;

[TestFixture]
public class ModelExtenderTests
{
    [Test]
    public void Can_Register_Types()
    {
        var options = new SwaggerGenOptions();

        SwaggerModelExtender.AddType<bool>().AddType<DateTime>().AddType<int>().Register(options);

        Assert.That(options.DocumentFilterDescriptors, Has.Count.EqualTo(1));

        Assert.That(options.DocumentFilterDescriptors[0].Type, Is.EqualTo(typeof(SwaggerModelExtender)));

        Assert.That(options.DocumentFilterDescriptors[0].Arguments[0], Is.EqualTo(new object[] {
            typeof(bool),
            typeof(DateTime),
            typeof(int)
        }));
    }
}
