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
        services.AddKeyedSingleton<IZIFDevice, PowerMaster8121>(SupportedZIFDevices.PowerMaster8121);
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