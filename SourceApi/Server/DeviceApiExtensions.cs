using RefMeterApi.Actions.Device;
using RefMeterApi.Controllers;

using SerialPortProxy;

using Swashbuckle.AspNetCore.SwaggerGen;

using WebSamDeviceApis.Actions.SerialPort;
using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Actions.VeinSource;
using WebSamDeviceApis.Controllers;

namespace WebSamDeviceApis;

public static class Configuration
{
    public static void UseDeviceApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(RefMeterController).Assembly.GetName().Name}.xml"));
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SerialPortConnection).Assembly.GetName().Name}.xml"));
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SourceController).Assembly.GetName().Name}.xml"));
    }

    public static void UseDeviceApi(this IEndpointRouteBuilder app)
    {
    }

    public static void UseDeviceApi(this IServiceCollection services, IConfiguration configuration)
    {
        switch (configuration["SourceType"])
        {
            case "simulated":
                services.AddSingleton<SimulatedSource>();
                services.AddSingleton<ISource>(x => x.GetRequiredService<SimulatedSource>());
                services.AddSingleton<ISimulatedSource>(x => x.GetRequiredService<SimulatedSource>());
                break;
            case "vein":
                services.AddSingleton(new VeinClient(new(), "localhost", 8080));
                services.AddSingleton<VeinSource>();
                services.AddSingleton<ISource>(x => x.GetRequiredService<VeinSource>());
                break;
            case "serial":
                services.AddSingleton<ISource, SerialPortSource>();
                break;
            default:
                throw new NotImplementedException($"Unknown SourceType: {configuration["SourceType"]}");
        }

        {
            var mockType = configuration["SerialPort:PortMockType"];

            if (!string.IsNullOrEmpty(mockType))
                services.AddSingleton(ctx => SerialPortConnection.FromMock(Type.GetType(mockType)!, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
            else
            {
                var portName = configuration["SerialPort:PortName"];

                if (string.IsNullOrEmpty(portName))
                    throw new NotSupportedException("either serial port name or port mock type must be set.");

                if (portName.Contains(':'))
                    services.AddSingleton(ctx => SerialPortConnection.FromNetwork(portName, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
                else
                    services.AddSingleton(ctx => SerialPortConnection.FromSerialPort(portName, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
            }

            services.AddSingleton<IRefMeter, SerialPortRefMeterDevice>();
        }
    }
}
