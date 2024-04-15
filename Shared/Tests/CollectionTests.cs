using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibraryTests;

class TestItem : IDatabaseObject
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

public abstract class CollectionTests : DatabaseTestCore
{
    private IObjectCollection<TestItem> Collection = null!;

    protected override async Task OnPostSetup()
    {
        Collection = Services.GetService<IObjectCollectionFactory<TestItem>>()!.Create("regular-collection", DatabaseCategories.Master);

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

        var item = new TestItem() { Name = "Test 1" };
        var added = await Collection.AddItem(item);

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
    }

    [Test]
    public async Task Will_Detect_Duplicate_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new TestItem() { Name = "Test 2" };

        await Collection.AddItem(item);

        Assert.That(() => Collection.AddItem(item).Wait(), Throws.Exception);
    }

    [Test]
    public async Task Can_Update_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new TestItem() { Name = "Test 3" };

        await Collection.AddItem(item);

        item.Name = "Test 4";

        var updated = await Collection.UpdateItem(item);

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
    }

    [Test]
    public void May_Not_Update_Unknown_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new TestItem() { Name = "Test 5" };

        Assert.ThrowsAsync<ArgumentException>(() => Collection.UpdateItem(item));
    }

    [Test]
    public async Task Can_Delete_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new TestItem() { Name = "Test 6" };

        await Collection.AddItem(item);

        var deleted = await Collection.DeleteItem(item.Id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted.Id, Is.EqualTo(item.Id));
            Assert.That(deleted.Name, Is.EqualTo("Test 6"));
        });

        var lookup = await Collection.GetItem(item.Id);

        Assert.That(lookup, Is.Null);
    }

    [Test]
    public void Can_Not_Delete_Non_Existing_Item()
    {
        Assert.That(Collection, Is.Not.Null);

        var item = new TestItem() { Name = "Test 7" };

        Assert.That(() => Collection.DeleteItem(item.Id).Wait(), Throws.Exception);
    }

    [Test]
    public async Task Can_Use_Case_Insensitive_Key()
    {
        Assert.That(Collection, Is.Not.Null);

        await Collection.AddItem(new TestItem() { Name = "Test 1", Data = "Data 1" });

        Assert.ThrowsAsync<TaskCanceledException>(async () => await Collection.AddItem(new TestItem() { Name = "Test 2", Data = "data 1" }));

        await Collection.AddItem(new TestItem() { Name = "Test 2", Data = "data 2" });
    }
}

[TestFixture]
public class InMemoryCollectionTests : CollectionTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbCollectionTests : CollectionTests
{
    protected override bool UseMongoDb { get; } = true;
}