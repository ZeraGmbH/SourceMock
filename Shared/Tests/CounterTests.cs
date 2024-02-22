using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Actions.Database;
using SharedLibrary.Models;

namespace SharedLibraryTests;


public abstract class CounterTests : DatabaseTestCore
{
    private ICounterCollection Collection = null!;

    protected override Task OnPostSetup()
    {
        Collection = Services.GetRequiredService<ICounterCollectionFactory>().Create(DatabaseCategories.Master);

        return Task.CompletedTask;
    }

    protected override void OnSetupServices(IServiceCollection services)
    {
    }

    [Test]
    public async Task Can_Increment_Counter()
    {
        var name1 = "A" + Guid.NewGuid().ToString("N");

        var one1 = await Collection.GetNextCounter(name1);
        Assert.That(one1, Is.EqualTo(1));

        var two1 = await Collection.GetNextCounter(name1);
        Assert.That(two1, Is.EqualTo(2));

        var name2 = "B" + Guid.NewGuid().ToString("N");

        var one2 = await Collection.GetNextCounter(name2);
        Assert.That(one2, Is.EqualTo(1));

        var two2 = await Collection.GetNextCounter(name2);
        Assert.That(two2, Is.EqualTo(2));

        var three1 = await Collection.GetNextCounter(name1);
        Assert.That(three1, Is.EqualTo(3));
    }
}

[TestFixture]
public class InMemoryCounterTests : CounterTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbCounterTests : CounterTests
{
    protected override bool UseMongoDb { get; } = true;
}