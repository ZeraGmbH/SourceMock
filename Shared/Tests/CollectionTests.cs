using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using DeviceApiSharedLibrary.Actions.Database;
using DeviceApiSharedLibrary.Models;
using DeviceApiSharedLibrary.Services;

namespace DeviceApiSharedLibraryTests;

class TestItem : DatabaseObject
{
    public string Name { get; set; } = null!;
}

class TestCollection : InMemoryCollection<TestItem>
{
    public TestCollection(ILogger<TestCollection> logger) : base(logger)
    {
    }
}

class MongoDbTestCollection : MongoDbCollection<TestItem>
{
    public override string CollectionName => "regular-collection";


    public MongoDbTestCollection(IMongoDbDatabaseService service, ILogger<MongoDbTestCollection> logger) : base(service, logger)
    {
    }
}

public abstract class CollectionTests
{
    protected abstract bool useMongoDb { get; }

    private IServiceProvider Services;

    [SetUp]
    public async Task Setup()
    {
        if (useMongoDb && Environment.GetEnvironmentVariable("EXECUTE_MONGODB_NUNIT_TESTS") != "yes")
        {
            Assert.Ignore("not runnig database tests");

            return;
        }

        var services = new ServiceCollection();

        services.AddLogging(l => l.AddProvider(NullLoggerProvider.Instance));

        if (useMongoDb)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.test.json")
                .Build();

            services.AddSingleton(configuration.GetSection("MongoDB").Get<MongoDbSettings>()!);

            services.AddSingleton<IMongoDbDatabaseService, MongoDbDatabaseService>();
            services.AddSingleton<IObjectCollection<TestItem>, MongoDbTestCollection>();
        }
        else
        {
            services.AddSingleton<IObjectCollection<TestItem>, TestCollection>();
        }

        Services = services.BuildServiceProvider();

        await Services.GetService<IObjectCollection<TestItem>>()!.RemoveAll();
    }

    [Test]
    public async Task Can_Add_Item()
    {
        var cut = Services.GetService<IObjectCollection<TestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new TestItem() { Name = "Test 1" };
        var added = await cut.AddItem(item, "autotest");

        Assert.Multiple(() =>
        {
            Assert.That(added.Id, Is.EqualTo(item.Id));
            Assert.That(added.Name, Is.EqualTo("Test 1"));
        });

        var lookup = await cut.GetItem(item.Id);

        Assert.That(lookup, Is.Not.SameAs(item));
        Assert.That(lookup, Is.Not.SameAs(added));

        Assert.Multiple(() =>
        {
            Assert.That(lookup.Id, Is.EqualTo(item.Id));
            Assert.That(lookup.Name, Is.EqualTo("Test 1"));
        });
    }

    [Test]
    public async Task Will_Detect_Duplicate_Item()
    {
        var cut = Services.GetService<IObjectCollection<TestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new TestItem() { Name = "Test 2" };

        await cut.AddItem(item, "autotest");

        Assert.That(() => cut.AddItem(item, "autotest").Wait(), Throws.Exception);
    }

    [Test]
    public async Task Can_Update_Item()
    {
        var cut = Services.GetService<IObjectCollection<TestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new TestItem() { Name = "Test 3" };

        await cut.AddItem(item, "autotest");

        item.Name = "Test 4";

        var updated = await cut.UpdateItem(item, "autotest");

        Assert.Multiple(() =>
        {
            Assert.That(updated.Id, Is.EqualTo(item.Id));
            Assert.That(updated.Name, Is.EqualTo("Test 4"));
        });

        var lookup = await cut.GetItem(item.Id);

        Assert.That(lookup, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(lookup.Id, Is.EqualTo(item.Id));
            Assert.That(lookup.Name, Is.EqualTo("Test 4"));
        });
    }

    [Test]
    public void May_Not_Update_Unknown_Item()
    {
        var cut = Services.GetService<IObjectCollection<TestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new TestItem() { Name = "Test 5" };

        Assert.ThrowsAsync<ArgumentException>(() => cut.UpdateItem(item, "autotest"));
    }

    [Test]
    public async Task Can_Delete_Item()
    {
        var cut = Services.GetService<IObjectCollection<TestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new TestItem() { Name = "Test 6" };

        await cut.AddItem(item, "autotest");

        var deleted = await cut.DeleteItem(item.Id, "autotest");

        Assert.Multiple(() =>
        {
            Assert.That(deleted.Id, Is.EqualTo(item.Id));
            Assert.That(deleted.Name, Is.EqualTo("Test 6"));
        });

        var lookup = await cut.GetItem(item.Id);

        Assert.That(lookup, Is.Null);
    }

    [Test]
    public void Can_Not_Delete_Non_Existing_Item()
    {
        var cut = Services.GetService<IObjectCollection<TestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new TestItem() { Name = "Test 7" };

        Assert.That(() => cut.DeleteItem(item.Id, "autotest").Wait(), Throws.Exception);
    }
}

[TestFixture]
public class InMemoryCollectionTests : CollectionTests
{
    protected override bool useMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbCollectionTests : CollectionTests
{
    protected override bool useMongoDb { get; } = true;
}