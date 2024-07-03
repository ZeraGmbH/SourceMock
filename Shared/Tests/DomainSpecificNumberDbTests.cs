using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Attributes;
using SharedLibrary.Actions.Database;
using SharedLibrary.DomainSpecific;
using SharedLibrary.Models;

namespace SharedLibraryTests;

public abstract class DomainSpecificNumberDbTests : DatabaseTestCore
{
    private IEntityStore Store = null!;

    public class NumberHolder
    {
        public Voltage Number { get; set; }
    }

    public class Entity : IDatabaseObject
    {
        [BsonId, NotNull, Required]
        public required string Id { get; set; }

        public List<NumberHolder> Numbers { get; set; } = [];

        public Current? Number { get; set; }
    }

    public interface IEntityStore
    {
        Task<Entity> Add(Entity result);

        Task<Entity?> Get(string id);
    }

    public class EntityStore(IObjectCollectionFactory<Entity> factory) : IEntityStore
    {
        private readonly IObjectCollection<Entity> _collection = factory.Create("sam-test-domain-specific-number", DatabaseCategories.Results);

        public IObjectCollection<Entity> Collection => _collection;

        public Task<Entity> Add(Entity entity) => _collection.AddItem(entity);


        public Task<Entity?> Get(string id) => _collection.GetItem(id);
    }

    protected override async Task OnPostSetup()
    {
        Store = Services.GetRequiredService<IEntityStore>();

        await ((EntityStore)Store).Collection.RemoveAll();
    }

    protected override void OnSetupServices(IServiceCollection services)
    {
        services.AddScoped<IEntityStore, EntityStore>();
    }

    [Test]
    public async Task Can_Write_And_Recover_Domain_Specific_Types()
    {
        var entity = new Entity
        {
            Id = "PeterPan1",
            Numbers = { new() { Number = new(220) }, new() { Number = new(230) }, new() { Number = new(240) } }
        };

        var added = await Store.Add(entity);

        var get = await Store.Get(entity.Id);

        Assert.Multiple(() =>
        {
            Assert.That(get, Is.Not.Null);
            Assert.That(get, Is.Not.SameAs(entity));
            Assert.That(get!.Id, Is.EqualTo(entity.Id));
            Assert.That(get.Number, Is.Null);
            Assert.That(get.Numbers, Has.Count.EqualTo(3));
            Assert.That((double)get.Numbers[0].Number, Is.EqualTo((double)entity.Numbers[0].Number));
            Assert.That((double)get.Numbers[1].Number, Is.EqualTo((double)entity.Numbers[1].Number));
            Assert.That((double)get.Numbers[2].Number, Is.EqualTo((double)entity.Numbers[2].Number));
        });
    }
}

[TestFixture]
public class InMemoryDomainSpecificNumberDbTests : DomainSpecificNumberDbTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbDomainSpecificNumberDbTests : DomainSpecificNumberDbTests
{
    protected override bool UseMongoDb { get; } = true;
}