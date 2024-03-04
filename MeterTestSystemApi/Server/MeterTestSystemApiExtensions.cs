using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary;
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
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(MeterTestSystemApiConfiguration).Assembly.GetName().Name}.xml"), true);

        /* Must add enumeration explicitly because it is only used as a dictionary key class. */
        SwaggerModelExtender.AddType<Amplifiers>().Register(options);
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
        /* All available implementations by type. */
        services.AddTransient<FallbackMeteringSystem>();
        services.AddTransient<MeterTestSystemMock>();
        services.AddTransient<SerialPortFGMeterTestSystem>();
        services.AddTransient<SerialPortMTMeterTestSystem>();

        /* Configure the factory. */
        var deviceType = configuration["SerialPort:DeviceType"];

        services.AddSingleton<IMeterTestSystemFactory>((ctx) =>
        {
            var factory = new MeterTestSystemFactory(ctx);

            switch (deviceType)
            {
                case "FG":
                    factory.Inititalize(new() { MeterTestSystemType = Models.Configuration.MeterTestSystemTypes.FG30x });
                    break;
                case "MT":
                    factory.Inititalize(new() { MeterTestSystemType = Models.Configuration.MeterTestSystemTypes.MT786 });
                    break;
                default:
                    factory.Inititalize(new() { MeterTestSystemType = Models.Configuration.MeterTestSystemTypes.Mock });
                    break;
            }

            return factory;
        });

        /* Access meter test system singlon through the factory. */
        services.AddTransient((ctx) => ctx.GetRequiredService<IMeterTestSystemFactory>().MeterTestSystem);

        /* Convenient accessors to the source, reference meter and error calculator - transient because these can change when reconfiguring the meter test system. */
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().ErrorCalculator);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().RefMeter);
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().Source);
    }
}
