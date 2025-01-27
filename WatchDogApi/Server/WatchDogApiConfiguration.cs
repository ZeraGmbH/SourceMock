using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

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