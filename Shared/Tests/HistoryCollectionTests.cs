using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson.Serialization.Attributes;
using DeviceApiSharedLibrary.Actions.Database;
using DeviceApiSharedLibrary.Models;
using DeviceApiSharedLibrary.Services;

namespace DeviceApiSharedLibraryTests;

[BsonIgnoreExtraElements]
class HistoryTestItem : DatabaseObject
{
    public string Name { get; set; } = null!;
}

class HistoryTestCollection : InMemoryHistoryCollection<HistoryTestItem>
{
    public HistoryTestCollection(ILogger<HistoryTestCollection> logger) : base(logger)
    {
    }
}

class MongoDbHistoryTestCollection : MongoDbHistoryCollection<HistoryTestItem>
{
    public override string CollectionName => "history-collection";


    public MongoDbHistoryTestCollection(IMongoDbDatabaseService service, ILogger<MongoDbHistoryTestCollection> logger) : base(service, logger)
    {
    }
}

public abstract class HistoryCollectionTests
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
            services.AddSingleton<IHistoryCollection<HistoryTestItem>, MongoDbHistoryTestCollection>();
        }
        else
        {
            services.AddSingleton<IHistoryCollection<HistoryTestItem>, HistoryTestCollection>();
        }

        services.AddSingleton<IObjectCollection<HistoryTestItem>>((s) => s.GetService<IHistoryCollection<HistoryTestItem>>()!);

        Services = services.BuildServiceProvider();

        await Services.GetService<IObjectCollection<HistoryTestItem>>()!.RemoveAll();
    }

    [Test]
    public async Task Can_Add_Item()
    {
        var cut = Services.GetService<IHistoryCollection<HistoryTestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 1" };
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

        var history = (await cut.GetHistory(item.Id)).ToArray();

        Assert.That(history, Is.Not.Null);
        Assert.That(history.Length, Is.EqualTo(1));

        var theOne = history[0];

        Assert.Multiple(() =>
        {
            Assert.That(theOne.Item.Id, Is.EqualTo(item.Id));
            Assert.That(theOne.Item.Name, Is.EqualTo("Test 1"));
            Assert.That(theOne.Version.ChangeCount, Is.EqualTo(1));
            Assert.That(theOne.Version.CreatedBy, Is.EqualTo("autotest"));
            Assert.That(theOne.Version.ModifiedBy, Is.EqualTo("autotest"));
            Assert.That(theOne.Version.ModifiedAt, Is.EqualTo(theOne.Version.CreatedAt));
        });
    }

    [Test]
    public async Task Will_Detect_Duplicate_Item()
    {
        var cut = Services.GetService<IHistoryCollection<HistoryTestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 2" };

        await cut.AddItem(item, "autotest");

        Assert.That(() => cut.AddItem(item, "autotest").Wait(), Throws.Exception);
    }

    [Test]
    public async Task Can_Update_Item()
    {
        var cut = Services.GetService<IHistoryCollection<HistoryTestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 3" };

        await cut.AddItem(item, "autotest");

        item.Name = "Test 4";

        var updated = await cut.UpdateItem(item, "updater");

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

        var history = (await cut.GetHistory(item.Id)).ToArray();

        Assert.That(history, Is.Not.Null);
        Assert.That(history.Length, Is.EqualTo(2));

        var theOne = history[0];

        Assert.Multiple(() =>
        {
            Assert.That(theOne.Item.Id, Is.EqualTo(item.Id));
            Assert.That(theOne.Item.Name, Is.EqualTo("Test 4"));
            Assert.That(theOne.Version.ChangeCount, Is.EqualTo(2));
            Assert.That(theOne.Version.CreatedBy, Is.EqualTo("autotest"));
            Assert.That(theOne.Version.ModifiedBy, Is.EqualTo("updater"));
            Assert.That(theOne.Version.ModifiedAt, Is.GreaterThanOrEqualTo(theOne.Version.CreatedAt));
        });

        var theFirst = history[1];

        Assert.Multiple(() =>
        {
            Assert.That(theFirst.Item.Id, Is.EqualTo(item.Id));
            Assert.That(theFirst.Item.Name, Is.EqualTo("Test 3"));
            Assert.That(theFirst.Version.ChangeCount, Is.EqualTo(1));
            Assert.That(theFirst.Version.CreatedBy, Is.EqualTo("autotest"));
            Assert.That(theFirst.Version.ModifiedBy, Is.EqualTo("autotest"));
            Assert.That(theFirst.Version.ModifiedAt, Is.EqualTo(theOne.Version.CreatedAt));
        });
    }

    [Test]
    public void May_Not_Update_Unknown_Item()
    {
        var cut = Services.GetService<IHistoryCollection<HistoryTestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 5" };

        Assert.ThrowsAsync<ArgumentException>(() => cut.UpdateItem(item, "autotest"));
    }

    [Test]
    public async Task Can_Delete_Item()
    {
        var cut = Services.GetService<IHistoryCollection<HistoryTestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 6" };

        await cut.AddItem(item, "autotest");

        var deleted = await cut.DeleteItem(item.Id, "autotest");

        Assert.Multiple(() =>
        {
            Assert.That(deleted.Id, Is.EqualTo(item.Id));
            Assert.That(deleted.Name, Is.EqualTo("Test 6"));
        });

        var lookup = await cut.GetItem(item.Id);

        Assert.That(lookup, Is.Null);

        var history = (await cut.GetHistory(item.Id)).ToArray();

        Assert.That(history, Is.Not.Null);
        Assert.That(history.Length, Is.EqualTo(useMongoDb ? 2 : 0));
    }

    [Test]
    public void Can_Not_Delete_Non_Existing_Item()
    {
        var cut = Services.GetService<IHistoryCollection<HistoryTestItem>>();

        Assert.That(cut, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 7" };

        Assert.That(() => cut.DeleteItem(item.Id, "autotest").Wait(), Throws.Exception);
    }
}

[TestFixture]
public class InMemoryHistoryCollectionTests : HistoryCollectionTests
{
    protected override bool useMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbHistoryCollectionTests : HistoryCollectionTests
{
    protected override bool useMongoDb { get; } = true;
}