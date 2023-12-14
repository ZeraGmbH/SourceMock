using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Actions.Database;
using SharedLibrary.ExceptionHandling;
using SharedLibrary.Models;
using SharedLibrary.Services;

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
            services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(MongoDbCollectionFactory<>));
            services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(MongoDbHistoryCollectionFactory<>));
        }
        else
        {
            services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(InMemoryCollectionFactory<>));
            services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(InMemoryHistoryCollectionFactory<>));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSharedLibrary(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDatabase(services, configuration);
        services.AddControllers(options =>
            {
                options.Filters.Add<DatabaseErrorFilter>();
            });
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSharedLibrary(this MvcOptions options)
    {
    }
}