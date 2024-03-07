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
using SharedLibrary;
using SourceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using SourceApi.Model.Configuration;
using SharedLibrary.Models;
using SourceApi.Actions.RestSource;

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

        services.AddTransient<IRestSource, RestSource>();
        services.AddTransient<ISerialPortFGSource, SerialPortFGSource>();
        services.AddTransient<ISerialPortMTSource, SerialPortMTSource>();
        services.AddTransient<ISimulatedSource, SimulatedSource>();
        services.AddTransient<ISourceMock, SimulatedSource>();

        /* Legacy configuration from setting files. */
        var useDatabase = configuration["UseDatabaseConfiguration"] == "yes";
        var deviceType = configuration["SerialPort:DeviceType"];

        if (!useDatabase)
            switch (configuration["SourceType"])
            {
                case "simulated":
                    if (integrated) break;

                    services.AddSingleton<ISource, SimulatedSource>();

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

        services.AddSingleton<ISerialPortConnectionFactory>((ctx) =>
        {
            var factory = new SerialPortConnectionFactory(ctx, ctx.GetRequiredService<ILogger<SerialPortConnectionFactory>>());

            if (!useDatabase)
            {
                var portName = configuration["SerialPort:PortName"];

                var meterSystemType = deviceType == "FG"
                    ? MeterTestSystemTypes.FG30x
                    : deviceType == "MT"
                    ? MeterTestSystemTypes.MT786
                    : MeterTestSystemTypes.Mock;

                var configurationType = configuration["SerialPort:UsePortMock"] == "yes"
                                        ? SerialPortConfigurationTypes.Mock
                                        : portName?.Contains(':') == true
                                        ? SerialPortConfigurationTypes.Network
                                        : SerialPortConfigurationTypes.Device;

                factory.Initialize(meterSystemType, meterSystemType == MeterTestSystemTypes.Mock ? null : new()
                {
                    ConfigurationType = configurationType,
                    EndPoint = portName!
                });
            }

            return factory;
        });

        services.AddTransient(ctx => ctx.GetRequiredService<ISerialPortConnectionFactory>().Connection);
    }
}