using DutApi.Actions;
using DutApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DutApi;

/// <summary>
/// 
/// </summary>
public static class DutApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void UseDutApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDeviceUnderTestFactory, ConnectionFactory>();

        services.AddKeyedTransient<IDeviceUnderTestConnection, ScpiConnection>(DutProtocolTypes.SCPIOverTCP);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void UseDutApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(DutApiConfiguration).Assembly.GetName().Name}.xml"), true);
    }
}
