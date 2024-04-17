using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Actions.Database;
using SharedLibrary.Actions.User;
using SharedLibrary.ExceptionHandling;
using SharedLibrary.Models;
using SharedLibrary.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedLibrary;

/// <summary>
/// 
/// </summary>
public static class SharedLibraryConfiguration
{
    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var mongoDb = configuration.GetSection("MongoDB").Get<MongoDbSettings>();

        if (!string.IsNullOrEmpty(mongoDb?.ServerName) && !string.IsNullOrEmpty(mongoDb?.DatabaseName))
        {
            services.AddSingleton(mongoDb);
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
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSharedLibrary(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDatabase(services, configuration);

        services.AddScoped<ICurrentUser, CurrentUser>();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSharedLibrary(this MvcOptions options)
    {
        options.Filters.Add<DatabaseErrorFilter>();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSharedLibrary(this SwaggerGenOptions options)
    {
        SwaggerModelExtender
            .AddType<SamDetailExtensions>()
            .AddType<SamDatabaseError>()
            .AddType<SamGlobalErrors>()
            .Register(options);
    }
}