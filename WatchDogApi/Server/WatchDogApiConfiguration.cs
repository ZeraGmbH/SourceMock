using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using WatchDogApi.Actions;
using WatchDogApi.Models;
using WatchDogApi.Services;

namespace WatchDogApi;

/// <summary>
/// 
/// </summary>
public static class WatchDogApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void UseWatchDogApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IWatchDogFactory, WatchDogFactory>();
        services.AddTransient<IWatchDogExecuter, WatchDogExecuter>();

        services.AddTransient(di => di.GetRequiredService<IWatchDogFactory>().WatchDog);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void UseWatchDogApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(WatchDogApiConfiguration).Assembly.GetName().Name}.xml"), true);
    }
}