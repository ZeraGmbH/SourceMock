using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using SourceApi.Actions.Source;
using SourceApi.Actions.VeinSource;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.SerialPort.MT768;
using Microsoft.Extensions.Logging;
using SourceApi.Actions.SerialPort;
using ZERA.WebSam.Shared;
using SourceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SourceApi.Model.Configuration;
using ZERA.WebSam.Shared.Models;
using SourceApi.Actions.RestSource;
using SourceApi.Actions.SimulatedSource;
using SourceApi.Actions;
using SourceApi.Controllers;

namespace SourceApi;

/// <summary>
/// 
/// </summary>
public static class SourceApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseSourceApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SourceApiConfiguration).Assembly.GetName().Name}.xml"), true);

        SwaggerModelExtender.AddType<SourceApiErrorCodes>().Register(options);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSourceApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// Add SourceApiExceptionFilter to local scope
    /// </summary>
    public static void UseSourceApi(this MvcOptions options)
    {
        options.Filters.Add<SourceApiExceptionFilter>();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSourceApi(this IServiceCollection services, IConfiguration configuration, bool integrated)
    {
        services.AddSingleton<ICapabilitiesMap, CapabilitiesMap>();

        services.AddTransient<IACSourceMock, ACSourceMock>();
        services.AddTransient<IDCSourceMock, DCSourceMock>();
        services.AddTransient<IRestDosage, RestDosage>();
        services.AddTransient<IRestSource, RestSource>();
        services.AddTransient<ISerialPortFGSource, SerialPortFGSource>();
        services.AddTransient<ISerialPortMTSource, SerialPortMTSource>();
        services.AddTransient<ISimulatedSource, SimulatedSource>();
        services.AddTransient<ISourceCapabilityValidator, SourceCapabilityValidator>();

        services.AddScoped<ISourceHealthUtils, SourceHealthUtils>();

        /* Legacy configuration from setting files. */
        var useDatabase = configuration["UseDatabaseConfiguration"] == "yes";
        var deviceType = configuration["SerialPort:DeviceType"];

        if (!useDatabase)
            switch (configuration["SourceType"])
            {
                case "simulated":
                    if (integrated) break;

                    services.AddSingleton<ISource, SimulatedSource>();
                    services.AddSingleton<ISourceCapabilityValidator, SourceCapabilityValidator>();

                    return;
                case "vein":
                    if (integrated)
                        throw new NotImplementedException($"Unknown SourceType: {configuration["SourceType"]}");

                    services.AddSingleton(new VeinClient(new(), "localhost", 8080));
                    services.AddSingleton<VeinSource>();
                    services.AddSingleton<ISource>(x => x.GetRequiredService<VeinSource>());

                    return;
                case "serial":
                    if (deviceType != "MT" && deviceType != "FG" && deviceType != "DeviceMock")
                        throw new NotImplementedException($"Unknown DeviceType: {deviceType}");

                    if (!integrated)
                        services.AddSingleton<ISource>(ctx => ctx.GetRequiredService<ISerialPortMTSource>());

                    break;
                default:
                    throw new NotImplementedException($"Unknown SourceType: {configuration["SourceType"]}");
            }

        services.AddKeyedSingleton<ISerialPortConnectionFactory>("MeterTestSystem", (ctx, key) =>
        {
            var factory = new SerialPortConnectionFactory(ctx, ctx.GetRequiredService<ILogger<SerialPortConnectionFactory>>());

            if (useDatabase) return factory;

            var portName = configuration["SerialPort:PortName"];

            var meterSystemType = deviceType == "FG"
                ? MeterTestSystemTypes.FG30x
                : deviceType == "MT"
                ? MeterTestSystemTypes.MT786
                : MeterTestSystemTypes.ACMock;

            var configurationType = configuration["SerialPort:UsePortMock"] == "yes"
                                    ? SerialPortConfigurationTypes.Mock
                                    : portName?.Contains(':') == true
                                    ? SerialPortConfigurationTypes.Network
                                    : SerialPortConfigurationTypes.Device;

            factory.Initialize(meterSystemType, meterSystemType == MeterTestSystemTypes.ACMock ? null : new()
            {
                ConfigurationType = configurationType,
                Endpoint = portName!
            });

            return factory;
        });

        services.AddKeyedTransient(KeyedService.AnyKey, (ctx, key) => ctx.GetRequiredKeyedService<ISerialPortConnectionFactory>(key).Connection);

        var restMock = configuration.GetValue<string>("UseSourceRestMock");

        if (restMock == "AC")
            services.AddKeyedSingleton<ISource, ACSourceMock>(SourceRestMockController.MockKey);
        else if (restMock == "DC")
            services.AddKeyedSingleton<ISource, DCSourceMock>(SourceRestMockController.MockKey);
        else
            services.AddKeyedSingleton<ISource, UnavailableSource>(SourceRestMockController.MockKey);

        services.AddKeyedTransient<IDosage>(DosageRestMockController.MockKey, (ctx, key) => ctx.GetRequiredKeyedService<ISource>(SourceRestMockController.MockKey));
    }
}