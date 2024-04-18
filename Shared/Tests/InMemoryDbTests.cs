using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;
using SharedLibrary.Actions.User;
using Moq;

namespace SharedLibraryTests;

[TestFixture]
public class InMemoryDbTests
{
    private ServiceProvider Services;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(InMemoryCollectionFactory<>));
        services.AddTransient(typeof(IObjectCollectionFactory<,>), typeof(InMemoryCollectionFactory<,>));
        services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(InMemoryHistoryCollectionFactory<>));
        services.AddTransient(typeof(IHistoryCollectionFactory<,>), typeof(InMemoryHistoryCollectionFactory<,>));

        services.AddTransient(typeof(NoopCollectionInitializer<>));

        services.AddSingleton(typeof(InMemoryCollection<,>.StateFactory));
        services.AddSingleton(typeof(InMemoryHistoryCollection<,>.InitializerFactory));

        var currentUser = new Mock<ICurrentUser>();

        services.AddSingleton(currentUser.Object);

        Services = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [Test]
    public async Task Object_Collection_Will_Survive_Scope_Change()
    {
        using (var scope = Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IObjectCollectionFactory<TestItem>>();
            var collection = factory.Create("coll", DatabaseCategories.Configuration);

            await collection.AddItem(new TestItem { Data = "theData", Name = "Peter", Id = Guid.NewGuid().ToString() });
        }

        using (var scope = Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IObjectCollectionFactory<TestItem>>();
            var collection = factory.Create("coll", DatabaseCategories.Configuration);

            var all = collection.CreateQueryable().ToArray();

            Assert.That(all, Has.Length.EqualTo(1));
            Assert.That(all[0].Name, Is.EqualTo("Peter"));
        }
    }

    [Test]
    public async Task History_Collection_Will_Survive_Scope_Change()
    {
        using (var scope = Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IHistoryCollectionFactory<TestItem>>();
            var collection = factory.Create("coll", DatabaseCategories.Configuration);

            await collection.AddItem(new TestItem { Data = "theData", Name = "Pan", Id = Guid.NewGuid().ToString() });
        }

        using (var scope = Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IHistoryCollectionFactory<TestItem>>();
            var collection = factory.Create("coll", DatabaseCategories.Configuration);

            var all = collection.CreateQueryable().ToArray();

            Assert.That(all, Has.Length.EqualTo(1));
            Assert.That(all[0].Name, Is.EqualTo("Pan"));
        }
    }
}