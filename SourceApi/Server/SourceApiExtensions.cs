using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using SourceApi.Actions.Source;
using SourceApi.Actions.VeinSource;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.SerialPort.MT768;
using SerialPortProxy;
using Microsoft.Extensions.Logging;
using SourceApi.Actions.SerialPort;
using SharedLibrary;
using SourceApi.Model;

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
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SourceApiConfiguration).Assembly.GetName().Name}.xml"));

        SwaggerModelExtender.AddType<SourceResult>().Register(options);
        SwaggerModelExtender.AddType<SourceApiErrorCodes>().Register(options);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSourceApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseSourceApi(this IServiceCollection services, IConfiguration configuration, bool serialOnly)
    {
        services.AddSingleton<ICapabilitiesMap, CapabilitiesMap>();

        var deviceType = configuration["SerialPort:DeviceType"];

        switch (configuration["SourceType"])
        {
            case "simulated":
                if (serialOnly)
                    throw new NotImplementedException($"Unknown SourceType: {configuration["SourceType"]}");

                services.AddSingleton<SimulatedSource>();
                services.AddSingleton<ISimulatedSource>(x => x.GetRequiredService<SimulatedSource>());
                services.AddSingleton<ISource>(x => x.GetRequiredService<SimulatedSource>());
                break;
            case "vein":
                if (serialOnly)
                    throw new NotImplementedException($"Unknown SourceType: {configuration["SourceType"]}");

                services.AddSingleton(new VeinClient(new(), "localhost", 8080));
                services.AddSingleton<VeinSource>();
                services.AddSingleton<ISource>(x => x.GetRequiredService<VeinSource>());
                break;
            case "serial":
                if (deviceType != "MT" && deviceType != "FG" && deviceType != "DeviceMock")
                    throw new NotImplementedException($"Unknown DeviceType: {deviceType}");

                if (!serialOnly)
                    services.AddSingleton<ISource>(ctx => ctx.GetRequiredService<ISerialPortMTSource>());
                else
                {
                    services.AddTransient<ISerialPortFGSource, SerialPortFGSource>();
                    services.AddTransient<ISerialPortMTSource, SerialPortMTSource>();
                }

                break;
            default:
                throw new NotImplementedException($"Unknown SourceType: {configuration["SourceType"]}");
        }


        {
            var usePortMock = configuration["SerialPort:UsePortMock"];

            if (usePortMock == "yes")
                switch (deviceType)
                {
                    case "FG":
                        services.AddSingleton(ctx => SerialPortConnection.FromMock<SerialPortFGMock>(ctx.GetRequiredService<ILogger<ISerialPortConnection>>()));
                        break;
                    default:
                        services.AddSingleton(ctx => SerialPortConnection.FromMock<SerialPortMTMock>(ctx.GetRequiredService<ILogger<ISerialPortConnection>>()));
                        break;
                }
            else
            {
                var portName = configuration["SerialPort:PortName"];

                if (string.IsNullOrEmpty(portName))
                    throw new NotSupportedException("serial port name must be set if not using serial port mock.");

                if (portName.Contains(':'))
                    services.AddSingleton(ctx => SerialPortConnection.FromNetwork(portName, ctx.GetRequiredService<ILogger<ISerialPortConnection>>()));
                else
                    services.AddSingleton(ctx => SerialPortConnection.FromSerialPort(portName, ctx.GetRequiredService<ILogger<ISerialPortConnection>>()));
            }
        }
    }
}