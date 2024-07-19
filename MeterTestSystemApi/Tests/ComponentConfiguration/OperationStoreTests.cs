using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.Tests;

namespace MeterTestSystemApiTests.ComponentConfiguration;

public abstract class OperationStoreTests : DatabaseTestCore
{
    private IProbingOperationStore Store = null!;

    protected override async Task OnPostSetup()
    {
        Store = Services.GetRequiredService<IProbingOperationStore>();

        await ((ProbingOperationStore)Store).Collection.RemoveAll();
    }

    protected override void OnSetupServices(IServiceCollection services)
    {
        services.AddSingleton<IProbingOperationStore, ProbingOperationStore>();
    }

    [Test]
    public async Task Can_Add_And_Update_A_Probing_Operation()
    {
        var created = new DateTime(2024, 7, 17, 14, 47, 22);
        var finished = new DateTime(2024, 7, 17, 14, 47, 43);

        var op = new ProbingOperation
        {
            Id = "OP1",
            Created = created,
            Finished = finished,
            Request = new() { Configuration = { EnableIPWatchDog = true, DCComponents = { DCComponents.CurrentSCG8, DCComponents.SPS } } },
            Result = new() { Configuration = { DCComponents = { DCComponents.SPS } }, Log = { "Test1", "Test2" } }
        };

        var added = await Store.Add(op);

        Assert.That(added.Request.Configuration.EnableCOM5003, Is.False);
        Assert.That(added.Request.Configuration.EnableIPWatchDog, Is.True);

        op.Request.Configuration.EnableCOM5003 = true;

        var updated = await Store.Update(op);

        Assert.That(updated.Request.Configuration.EnableCOM5003, Is.True);
        Assert.That(updated.Request.Configuration.EnableIPWatchDog, Is.True);

        var read = await Store.Get("OP1");

        Assert.That(read, Is.Not.Null);
        Assert.That(read.Request.Configuration.EnableCOM5003, Is.True);
        Assert.That(read.Request.Configuration.EnableIPWatchDog, Is.True);
        Assert.That(read.Request.Configuration.DCComponents, Is.EqualTo(new[] { DCComponents.CurrentSCG8, DCComponents.SPS }));
        Assert.That(read.Result.Configuration.DCComponents, Is.EqualTo(new[] { DCComponents.SPS }));
        Assert.That(read.Result.Log, Is.EqualTo(new[] { "Test1", "Test2" }));
    }
}

[TestFixture]
public class InMemoryDbOperationStoreTests : OperationStoreTests
{
    protected override bool UseMongoDb => false;
}

[TestFixture]
public class MongoDbDbOperationStoreTests : OperationStoreTests
{
    protected override bool UseMongoDb => false;
}