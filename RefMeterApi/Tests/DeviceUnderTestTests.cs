using DeviceApiLib.Actions.Database;
using DeviceApiSharedLibrary.Actions.Database;
using DeviceApiSharedLibrary.Models;
using DeviceApiSharedLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Models;
using RefMeterApi.Services;

namespace RefMeterApiTests;

public abstract class DeviceUnderTestTests
{
    protected abstract bool UseMongoDb { get; }

    private IServiceProvider Services;

    private IDeviceUnderTestStorage _storage;

    [SetUp]
    public async Task Setup()
    {
        if (UseMongoDb && Environment.GetEnvironmentVariable("EXECUTE_MONGODB_NUNIT_TESTS") != "yes")
        {
            Assert.Ignore("not runnig database tests");

            return;
        }

        var services = new ServiceCollection();

        services.AddLogging(l => l.AddProvider(NullLoggerProvider.Instance));

        services.AddSingleton<IDeviceUnderTestStorage, DeviceUnderTestStorage>();

        if (UseMongoDb)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.test.json")
                .Build();

            services.AddSingleton(configuration.GetSection("MongoDB").Get<MongoDbSettings>()!);

            services.AddSingleton<IMongoDbDatabaseService, MongoDbDatabaseService>();
            services.AddSingleton(typeof(IHistoryCollectionFactory<>), typeof(MongoDbHistoryCollectionFactory<>));
        }
        else
        {
            services.AddSingleton(typeof(IHistoryCollectionFactory<>), typeof(InMemoryHistoryCollectionFactory<>));
        }

        Services = services.BuildServiceProvider();

        _storage = Services.GetService<IDeviceUnderTestStorage>()!;

        await ((DeviceUnderTestStorage)_storage).Collection.RemoveAll();
    }

    [Test]
    public async Task Can_Add_A_New_Device_Under_Test()
    {
        var added = await _storage.Add(new NewDeviceUnderTest
        {
            Name = "Test1"
        }, "autotest");

        Assert.That(added, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(added.Id.Length, Is.GreaterThan(0));
            Assert.That(added.Name, Is.EqualTo("Test1"));
        });
        var dut = await _storage.Get(added.Id);

        Assert.That(dut, Is.Not.Null);
        Assert.That(dut.Name, Is.EqualTo("Test1"));
    }
}

[TestFixture]
public class InMemoryDeviceUnderTestTests : DeviceUnderTestTests
{
    protected override bool UseMongoDb { get; } = false;
}

[TestFixture]
public class MongoDbDeviceUnderTestTests : DeviceUnderTestTests
{
    protected override bool UseMongoDb { get; } = true;
}