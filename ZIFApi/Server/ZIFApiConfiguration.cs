using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZIFApi.Actions;
using ZIFApi.Models;

namespace ZIFApi;

/// <summary>
/// 
/// </summary>
public static class ZIFApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void UseZIFApi(this IServiceCollection services, IConfiguration configuration)
    {
        // All known ZIF implementations
        services.AddKeyedTransient<IZIFProtocol, PowerMaster8121>(SupportedZIFProtocols.PowerMaster8121);

        // Used to initialize devices from configuration.
        services.AddSingleton<IZIFDevicesFactory, ZIFDevicesFactory>();

        // Register all sockets.
        services.AddTransient(di => di.GetRequiredService<IZIFDevicesFactory>().Devices);
        services.AddTransient(di => di.GetRequiredService<IZIFDevice[]>().FirstOrDefault()!);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void UseZIFApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ZIFApiConfiguration).Assembly.GetName().Name}.xml"), true);
    }
}