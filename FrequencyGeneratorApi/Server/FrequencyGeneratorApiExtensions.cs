using MeteringSystemApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MeteringSystemApi;

/// <summary>
/// 
/// </summary>
public static class MeteringSystemApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseMeteringSystemApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(MeteringSystemApiConfiguration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseMeteringSystemApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseMeteringSystemApi(this IServiceCollection services, IConfiguration configuration)
    {
        var deviceType = configuration["SerialPort:DeviceType"];

        switch (deviceType)
        {
            case "FG":
                services.AddSingleton<IMeterTestSystem, SerialPortFGMeteringSystem>();
                break;
            case "MT":
                services.AddSingleton<IMeterTestSystem, SerialPortMTMeteringSystem>();
                break;
            default:
                return;
        }

        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().ErrorCalculator);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().RefMeter);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().Source);
    }
}
