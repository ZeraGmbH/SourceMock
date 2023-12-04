using MeteringSystemApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MeteringSystemApi;

/// <summary>
/// 
/// </summary>
public static class Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseMeteringSystemApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Configuration).Assembly.GetName().Name}.xml"));
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

        if (deviceType != "MT" && deviceType != "FG")
            throw new NotImplementedException($"Unknown DeviceType: {deviceType}");

        switch (deviceType)
        {
            case "FG":
                services.AddSingleton<IMeteringSystem, SerialPortFGMeteringSystem>();
                break;
            default:
                services.AddSingleton<IMeteringSystem, SerialPortMTMeteringSystem>();
                break;
        }
    }
}
