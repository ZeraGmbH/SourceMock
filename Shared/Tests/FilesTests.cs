using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;
using System.Text;

namespace SharedLibraryTests;

class TestFile
{
    public string MimeType { get; set; } = null!;
}

public abstract class FilesTests : DatabaseTestCore
{
    private IFilesCollection<TestFile> Collection = null!;

    protected override async Task OnPostSetup()
    {
        Collection = Services.GetRequiredService<IFilesCollectionFactory<TestFile>>().Create("file-collection", DatabaseCategories.Master);

        await Collection.RemoveAll();
    }

    protected override void OnSetupServices(IServiceCollection services)
    {
    }

    [Test]
    public async Task Can_Add_And_Query_A_File()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Dr. Jochen Manns"));

        var id = await Collection.AddFile("TheFile.txt", new TestFile { MimeType = "text/plain" }, stream);

        Assert.That(id, Is.Not.Null);

        var info = await Collection.FindFile(id);

        Assert.That(info, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(info.Id, Is.EqualTo(id));
            Assert.That(info.Name, Is.EqualTo("TheFile.txt"));
            Assert.That(info.Length, Is.EqualTo(16));
            Assert.That(info.Meta.MimeType, Is.EqualTo("text/plain"));
            Assert.That(info.UserId, Is.EqualTo("autotest"));
        });

        using var reader = new StreamReader(await Collection.Open(id));

        Assert.That(reader.ReadToEnd(), Is.EqualTo("Dr. Jochen Manns"));

        await Collection.DeleteFile(id);

        Assert.That(await Collection.FindFile(id), Is.Null);
    }
}

[TestFixture]
public class InMemoryFilesTests : FilesTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbFilesTests : FilesTests
{
    protected override bool UseMongoDb { get; } = true;
}