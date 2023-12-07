using MeterTestSystemApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MeterTestSystemApi;

/// <summary>
/// 
/// </summary>
public static class MeterTestSystemApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseMeterTestSystemApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(MeterTestSystemApiConfiguration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseMeterTestSystemApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseMeterTestSystemApi(this IServiceCollection services, IConfiguration configuration)
    {
        var deviceType = configuration["SerialPort:DeviceType"];

        switch (deviceType)
        {
            case "FG":
                services.AddSingleton<IMeterTestSystem, SerialPortFGMeterTestSystem>();
                break;
            case "MT":
                services.AddSingleton<IMeterTestSystem, SerialPortMTMeterTestSystem>();
                break;
            default:
                return;
        }

        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().ErrorCalculator);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().RefMeter);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().Source);
    }
}
