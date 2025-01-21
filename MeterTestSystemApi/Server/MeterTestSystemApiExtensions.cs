using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions;
using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Controllers;
using MeterTestSystemApi.Models;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.Models;

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
        SwaggerModelExtender
            .AddType<Amplifiers>()
            .AddType<SerialProbe>()
            .AddType<SerialProbeOverTcp>()
            .Register(options);
    }

    /// <summary>
    /// Apply endpoint routing to the configuration - this includes especially SignalR services.
    /// </summary>
    /// <param name="app">Configuration builder instance.</param>
    public static void UseMeterTestSystemApi(this IEndpointRouteBuilder app)
    {
    }

    private static T ForbidDisposable<T>(T instance, IServiceProvider services)
    {
        /* As expected. */
        if (instance is not IDisposable) return instance;

        /* Report and terminate. */
        services.GetRequiredService<ILogger<T>>().LogCritical("{Type} must not implement IDisposable", instance.GetType().FullName);

        throw new InvalidDataException("Implementation of IDisposable forbidden - please contact developer.");
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
        services.AddTransient<MeterTestSystemAcMock>();
        services.AddTransient<MeterTestSystemDcMock>();
        services.AddTransient<RestMeterTestSystem>();
        services.AddTransient<SerialPortFGMeterTestSystem>();
        services.AddTransient<SerialPortMTMeterTestSystem>();

        /* Configure the factory. */
        services.AddTransient<MeterTestSystemFactory>();

        services.AddSingleton<IMeterTestSystemFactory>((ctx) =>
        {
            var factory = ctx.GetRequiredService<MeterTestSystemFactory>();

            if (configuration["UseDatabaseConfiguration"] == "yes") return factory;

            switch (configuration["SerialPort:DeviceType"])
            {
                case "FG":
                    factory.Initialize(new() { MeterTestSystemType = MeterTestSystemTypes.FG30x });
                    break;
                case "MT":
                    factory.Initialize(new() { MeterTestSystemType = MeterTestSystemTypes.MT786 });
                    break;
                default:
                    factory.Initialize(new() { MeterTestSystemType = MeterTestSystemTypes.ACMock });
                    break;
            }

            return factory;
        });

        /* Access meter test system singleton through the factory. */
        services.AddTransient((ctx) => ctx.GetRequiredService<IMeterTestSystemFactory>().MeterTestSystem);

        /* 
            WARNING

            These convienent registration should be transient since the implemenation may
            change during the life time of the service - although this is not very likely
            during work it may be a problem when in the startup phase there are some 
            early bindings to these services.

            But transient services will be disposed by the dependency injection system when
            the created instance is no longer used. So it's strongly forbidden to implement
            IDisposable on any of the services registered here. This restriction is based
            on the class itself not interfaces implemented or used here.
        */

        /* Convenient accessors to the source, reference meter and error calculator - transient because these can change when reconfiguring the meter test system. */
        services.AddTransient(di => di.GetRequiredService<IMeterTestSystem>().ErrorCalculators);

        services.AddTransient(di => ForbidDisposable(di.GetRequiredService<IErrorCalculator[]>()[0], di));
        services.AddTransient(di => ForbidDisposable(di.GetRequiredService<IMeterTestSystem>().RefMeter, di));
        services.AddTransient(di => ForbidDisposable(di.GetRequiredService<IMeterTestSystem>().Source, di));

        /* Probing core. */
        services.AddSingleton<IProbeConfigurationService, ProbeConfigurationService>();
        services.AddScoped<IConfigurationProbePlan, ConfigurationProbePlan>();
        services.AddScoped<IProbingOperationStore, ProbingOperationStore>();

        /* Probing algorithms. */
        services.AddTransient<ISerialPortConnectionForProbing, SerialPortConnectionForProbing>();
        services.AddKeyedScoped<IProbeExecutor, ProbeSerialPort>(typeof(SerialProbe));
        services.AddKeyedScoped<IProbeExecutor, ProbeSerialPortOverTcp>(typeof(SerialProbeOverTcp));
        services.AddKeyedScoped<ISerialPortProbeExecutor, ESxBSerialPortProbing>(SerialProbeProtocols.ESxB);
        services.AddKeyedScoped<ISerialPortProbeExecutor, FGSerialPortProbing>(SerialProbeProtocols.FG30x);
        services.AddKeyedScoped<ISerialPortProbeExecutor, MTSerialPortProbing>(SerialProbeProtocols.MT768);
        services.AddKeyedScoped<ISerialPortProbeExecutor, ZIFSerialPortProbing>(SerialProbeProtocols.PM8181);

        if (configuration.GetValue<bool>("UseMeterTestSystemRestMock") == true)
            services.AddKeyedSingleton<IMeterTestSystem, FallbackMeteringSystem>(MeterTestSystemRestController.MockKey);
        else
            services.AddKeyedSingleton<IMeterTestSystem, FallbackMeteringSystem>(MeterTestSystemRestController.MockKey);

        services.AddSingleton<ICustomSerialPortFactory, CustomSerialPortFactory>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public static Task StartMeterTestSystemApiAsync(this IServiceProvider services) => Task.WhenAll(
        services.StartServiceAsync<IProbingOperationStore>()
    );
}
