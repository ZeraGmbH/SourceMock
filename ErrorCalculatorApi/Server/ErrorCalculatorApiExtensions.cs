using ErrorCalculatorApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ErrorCalculatorApi;

/// <summary>
/// 
/// </summary>
public static class Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseErrorCalculatorApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Configuration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseErrorCalculatorApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseErrorCalculatorApi(this IServiceCollection services, IConfiguration configuration)
    {
        var deviceType = configuration["SerialPort:DeviceType"];

        if (deviceType != "MT" && deviceType != "FG")
            return;

        switch (deviceType)
        {
            case "FG":
                services.AddSingleton<IErrorCalculator, SerialPortFGErrorCalculator>();
                break;
            case "MT":
                services.AddSingleton<IErrorCalculator, SerialPortMTErrorCalculator>();
                break;
        }
    }
}
