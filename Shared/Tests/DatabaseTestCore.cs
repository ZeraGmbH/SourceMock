using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedLibrary.Actions.Database;
using SharedLibrary.Actions.User;
using SharedLibrary.Models;
using SharedLibrary.Services;

namespace SharedLibraryTests;

public abstract class DatabaseTestCore
{
    protected abstract bool UseMongoDb { get; }

    protected ServiceProvider Services;

    protected string UserId;

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    protected abstract void OnSetupServices(IServiceCollection services);

    protected abstract Task OnPostSetup();

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

        OnSetupServices(services);

        if (UseMongoDb)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.test.json")
                .Build();

            services.AddSingleton(configuration.GetSection("MongoDB").Get<MongoDbSettings>()!);
            services.AddSingleton<IMongoDbDatabaseService, MongoDbDatabaseService>();

            services.AddTransient(typeof(IFileCollectionFactory<,>), typeof(MongoDbFileFactory<,>));
            services.AddTransient(typeof(IFileCollectionFactory<>), typeof(MongoDbFileFactory<>));
            services.AddTransient(typeof(IHistoryCollectionFactory<,>), typeof(MongoDbHistoryCollectionFactory<,>));
            services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(MongoDbHistoryCollectionFactory<>));
            services.AddTransient(typeof(IObjectCollectionFactory<,>), typeof(MongoDbCollectionFactory<,>));
            services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(MongoDbCollectionFactory<>));

            services.AddTransient<ICounterCollectionFactory, MongoDbCountersFactory>();
        }
        else
        {
            services.AddTransient(typeof(IFileCollectionFactory<,>), typeof(InMemoryFilesFactory<,>));
            services.AddTransient(typeof(IFileCollectionFactory<>), typeof(InMemoryFilesFactory<>));
            services.AddTransient(typeof(IHistoryCollectionFactory<,>), typeof(InMemoryHistoryCollectionFactory<,>));
            services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(InMemoryHistoryCollectionFactory<>));
            services.AddTransient(typeof(IObjectCollectionFactory<,>), typeof(InMemoryCollectionFactory<,>));
            services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(InMemoryCollectionFactory<>));

            services.AddTransient(typeof(NoopInitializer<>));
            services.AddTransient(typeof(NoopFileInitializer<>));

            services.AddSingleton(typeof(InMemoryCollection<,>.StateFactory));
            services.AddSingleton(typeof(InMemoryHistoryCollection<,>.InitializerFactory));

            services.AddSingleton(typeof(InMemoryFiles<,>.StateFactory));

            services.AddTransient<ICounterCollectionFactory, InMemoryCountersFactory>();
        }

        UserId = "autotest";

        var userMock = new Mock<ICurrentUser>();

        userMock.Setup(u => u.User).Returns(() =>
            new ClaimsPrincipal([
                new ClaimsIdentity([
                    new Claim(ClaimTypes.NameIdentifier, UserId)])]));

        services.AddSingleton(userMock.Object);

        Services = services.BuildServiceProvider();

        await OnPostSetup();
    }
}
