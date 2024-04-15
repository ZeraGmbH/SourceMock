using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Attributes;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;

namespace SharedLibraryTests;

public abstract class ScopedTests : DatabaseTestCore
{
    public class TheStoreItem : IDatabaseObject
    {
        [BsonId]
        public string Id { get; set; } = null!;
    }


    public interface ITheStore
    {
        string Id { get; }

        Task<TheStoreItem> Create(string userId);
    }


    public class TheStoreCommon : CollectionInitializer<TheStoreItem>
    {
        public string Id { get; private set; } = null!;

        protected override Task OnInitialize(IObjectCollection<TheStoreItem> collection)
        {
            Assert.That(Id, Is.Null);

            Id = Guid.NewGuid().ToString();

            return Task.CompletedTask;
        }
    }

    public class TheStore(IObjectCollectionFactory<TheStoreItem, TheStoreCommon> factory) : ITheStore
    {
        public string Id => _collection.Common.Id;

        private readonly IObjectCollection<TheStoreItem, TheStoreCommon> _collection = factory.Create("the-store-test", DatabaseCategories.Master);

        public Task<TheStoreItem> Create(string userId) => _collection.AddItem(new() { Id = Guid.NewGuid().ToString() }, userId);
    }

    protected override Task OnPostSetup()
    {
        return Task.CompletedTask;
    }

    protected override void OnSetupServices(IServiceCollection services)
    {
        services.AddScoped<ITheStore, TheStore>();
        services.AddSingleton<TheStoreCommon>();
    }

    [Test]
    public void Can_Used_Scoped_Service_Proxy()
    {
        using var scope1 = Services.CreateScope();

        var services1 = scope1.ServiceProvider;

        var store1 = services1.GetRequiredService<ITheStore>();
        var store2 = services1.GetRequiredService<ITheStore>();

        Assert.That(store1, Is.SameAs(store2));
        Assert.That(store1.Id, Is.Not.Null);

        using var scope2 = Services.CreateScope();

        var services2 = scope2.ServiceProvider;

        var store3 = services2.GetRequiredService<ITheStore>();

        Assert.That(store3, Is.Not.SameAs(store1));

        Assert.That(store3.Id, Is.EqualTo(store1.Id));
    }
}

[TestFixture]
public class InMemoryScopedTests : ScopedTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbScopedTests : ScopedTests
{
    protected override bool UseMongoDb { get; } = true;
}