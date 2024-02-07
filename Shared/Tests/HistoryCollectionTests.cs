using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Attributes;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;
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
    [BsonId, Required, NotNull, MinLength(1)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public string Data { get; set; } = null!;
}

public abstract class HistoryCollectionTests : DatabaseTestCore
{
    private IHistoryCollection<HistoryTestItem> Collection = null!;

    protected override async Task OnPostSetup()
    {
        Collection = Services.GetService<IHistoryCollectionFactory<HistoryTestItem>>()!.Create("history-collection");

        await Collection.CreateIndex("karl", i => i.Data, caseSensitive: false);

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
        Assert.That(history, Has.Length.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(history[0].ChangeCount, Is.EqualTo(1));
            Assert.That(history[0].CreatedBy, Is.EqualTo("autotest"));
            Assert.That(history[0].ModifiedBy, Is.EqualTo("autotest"));
            Assert.That(history[0].ModifiedAt, Is.EqualTo(history[0].CreatedAt));
        });

        var theOne = await Collection.GetHistoryItem(item.Id, history[0].ChangeCount);

        Assert.Multiple(() =>
        {
            Assert.That(theOne.Id, Is.EqualTo(item.Id));
            Assert.That(theOne.Name, Is.EqualTo("Test 1"));
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
        Assert.That(history, Has.Length.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(history[0].ChangeCount, Is.EqualTo(2));
            Assert.That(history[0].CreatedBy, Is.EqualTo("autotest"));
            Assert.That(history[0].ModifiedBy, Is.EqualTo("updater"));
            Assert.That(history[0].ModifiedAt, Is.GreaterThanOrEqualTo(history[0].CreatedAt));

            Assert.That(history[1].ChangeCount, Is.EqualTo(1));
            Assert.That(history[1].CreatedBy, Is.EqualTo("autotest"));
            Assert.That(history[1].ModifiedBy, Is.EqualTo("autotest"));
            Assert.That(history[1].ModifiedAt, Is.EqualTo(history[0].CreatedAt));
        });

        var theOne = await Collection.GetHistoryItem(item.Id, history[0].ChangeCount);

        Assert.Multiple(() =>
        {
            Assert.That(theOne.Id, Is.EqualTo(item.Id));
            Assert.That(theOne.Name, Is.EqualTo("Test 4"));
        });

        var theFirst = await Collection.GetHistoryItem(item.Id, history[1].ChangeCount);

        Assert.Multiple(() =>
        {
            Assert.That(theFirst.Id, Is.EqualTo(item.Id));
            Assert.That(theFirst.Name, Is.EqualTo("Test 3"));
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


    [Test]
    public async Task Can_Use_Case_Insensitive_Key()
    {
        Assert.That(Collection, Is.Not.Null);

        await Collection.AddItem(new HistoryTestItem() { Name = "Test 1", Data = "Data 1" }, "autotest");

        Assert.ThrowsAsync<TaskCanceledException>(async () => await Collection.AddItem(new HistoryTestItem() { Name = "Test 2", Data = "data 1" }, "autotest"));

        await Collection.AddItem(new HistoryTestItem() { Name = "Test 2", Data = "data 2" }, "autotest");
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