using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson.Serialization.Attributes;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;
using SharedLibrary.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharedLibraryTests;

[BsonIgnoreExtraElements]
class HistoryTestItem : IDatabaseObject
{
    /// <summary>
    /// Unique identifer of the object which can be used
    /// as a primary key. Defaults to a new Guid.
    /// </summary>
    [BsonId]
    [Required]
    [NotNull]
    [MinLength(1)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
}

public abstract class HistoryCollectionTests : DatabaseTestCore
{
    private IHistoryCollection<HistoryTestItem> Collection = null!;

    protected override async Task OnPostSetup()
    {
        Collection = Services.GetService<IHistoryCollectionFactory<HistoryTestItem>>()!.Create("history-collection");

        await Collection.RemoveAll();
    }

    protected override void OnSetupServices(IServiceCollection services)
    {
    }

    [Test]
    public async Task Can_Add_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 1" };
        var added = await Collection.AddItem(item, "autotest");

        Assert.Multiple(() =>
        {
            Assert.That(added.Id, Is.EqualTo(item.Id));
            Assert.That(added.Name, Is.EqualTo("Test 1"));
        });

        var lookup = await Collection.GetItem(item.Id);

        Assert.That(lookup, Is.Not.SameAs(item));
        Assert.That(lookup, Is.Not.SameAs(added));

        Assert.Multiple(() =>
        {
            Assert.That(lookup.Id, Is.EqualTo(item.Id));
            Assert.That(lookup.Name, Is.EqualTo("Test 1"));
        });

        var history = (await Collection.GetHistory(item.Id)).ToArray();

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
        Assert.That(Collection, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 2" };

        await Collection.AddItem(item, "autotest");

        Assert.That(() => Collection.AddItem(item, "autotest").Wait(), Throws.Exception);
    }

    [Test]
    public async Task Can_Update_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 3" };

        await Collection.AddItem(item, "autotest");

        item.Name = "Test 4";

        var updated = await Collection.UpdateItem(item, "updater");

        Assert.Multiple(() =>
        {
            Assert.That(updated.Id, Is.EqualTo(item.Id));
            Assert.That(updated.Name, Is.EqualTo("Test 4"));
        });

        var lookup = await Collection.GetItem(item.Id);

        Assert.That(lookup, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(lookup.Id, Is.EqualTo(item.Id));
            Assert.That(lookup.Name, Is.EqualTo("Test 4"));
        });

        var history = (await Collection.GetHistory(item.Id)).ToArray();

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
        Assert.That(Collection, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 5" };

        Assert.ThrowsAsync<ArgumentException>(() => Collection.UpdateItem(item, "autotest"));
    }

    [Test]
    public async Task Can_Delete_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 6" };

        await Collection.AddItem(item, "autotest");

        var deleted = await Collection.DeleteItem(item.Id, "autotest");

        Assert.Multiple(() =>
        {
            Assert.That(deleted.Id, Is.EqualTo(item.Id));
            Assert.That(deleted.Name, Is.EqualTo("Test 6"));
        });

        var lookup = await Collection.GetItem(item.Id);

        Assert.That(lookup, Is.Null);

        var history = (await Collection.GetHistory(item.Id)).ToArray();

        Assert.That(history, Is.Not.Null);
        Assert.That(history.Length, Is.EqualTo(UseMongoDb ? 2 : 0));
    }

    [Test]
    public void Can_Not_Delete_Non_Existing_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new HistoryTestItem() { Name = "Test 7" };

        Assert.That(() => Collection.DeleteItem(item.Id, "autotest").Wait(), Throws.Exception);
    }
}

[TestFixture]
public class InMemoryHistoryCollectionTests : HistoryCollectionTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbHistoryCollectionTests : HistoryCollectionTests
{
    protected override bool UseMongoDb { get; } = true;
}