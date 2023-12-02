using FrequencyGeneratorApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FrequencyGeneratorApi;

/// <summary>
/// 
/// </summary>
public static class Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseFrequencyGeneratorApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Configuration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseFrequencyGeneratorApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseFrequencyGeneratorApi(this IServiceCollection services, IConfiguration configuration)
    {
        var deviceType = configuration["SerialPort:DeviceType"];

        if (deviceType != "MT" && deviceType != "FG")
            throw new NotImplementedException($"Unknown DeviceType: {deviceType}");

        switch (deviceType)
        {
            case "FG":
                services.AddSingleton<IFrequencyGenerator, SerialPortFGFrequencyGenerator>();
                break;
            default:
                services.AddSingleton<IFrequencyGenerator, SerialPortMTFrequencyGenerator>();
                break;
        }
    }
}
