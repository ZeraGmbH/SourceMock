using MeterTestSystemApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MeterTestSystemApi;

/// <summary>
/// Helper methods to configure web server and esp. dependency injection.
/// </summary>
public static class MeterTestSystemApiConfiguration
{
    /// <summary>
    /// Apply additional configuration to OpenAPI core.
    /// </summary>
    /// <param name="options">Swagger options.</param>
    public static void UseMeterTestSystemApi(this SwaggerGenOptions options)
    {
        /* Add all XML documentation to OpenAPI schema. */
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(MeterTestSystemApiConfiguration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// Apply endpoint routing to the configuration - this includes especially SignalR services.
    /// </summary>
    /// <param name="app">Configuration builder instance.</param>
    public static void UseMeterTestSystemApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// Configure the dependency injection.
    /// </summary>
    /// <param name="services">Dependency injection configurator.</param>
    /// <param name="configuration">Overall configuration of the web server.</param>
    public static void UseMeterTestSystemApi(this IServiceCollection services, IConfiguration configuration)
    {
        /* Create the correct implemetation of the meter test system - only serial port communication is supported. */
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
                /* Maybe the web server configuration does not need the meter test system abstraction at all. */
                return;
        }

        /* Convinient accessors to the source, reference meter and error calculator - transient because these can change when reconfiguring the meter test system. */
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().ErrorCalculator);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().RefMeter);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().Source);
    }
}
