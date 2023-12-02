using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Controllers;

using FrequencyGeneratorApi.Actions.Device;
using FrequencyGeneratorApi.Models;

using RefMeterApi.Actions.Device;
using RefMeterApi.Controllers;

using SerialPortProxy;

using Swashbuckle.AspNetCore.SwaggerGen;

using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;
using SourceApi.Actions.VeinSource;
using SourceApi.Controllers;

namespace WebSamDeviceApis;

public static class Configuration
{
    public static void UseDeviceApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ErrorMeasurementController).Assembly.GetName().Name}.xml"));
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(FrequencyGeneratorCapabilities).Assembly.GetName().Name}.xml"));
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(RefMeterController).Assembly.GetName().Name}.xml"));
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SerialPortConnection).Assembly.GetName().Name}.xml"));
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(SourceController).Assembly.GetName().Name}.xml"));
    }

    public static void UseDeviceApi(this IEndpointRouteBuilder app)
    {
    }

    public static void UseDeviceApi(this IServiceCollection services, IConfiguration configuration)
    {
        var deviceType = configuration["SerialPort:DeviceType"];

        if (deviceType != "MT" && deviceType != "FG")
            throw new NotImplementedException($"Unknown DeviceType: {deviceType}");

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
                switch (deviceType)
                {
                    case "FG":
                        services.AddSingleton<ISource, SerialPortFGSource>();
                        break;
                    default:
                        services.AddSingleton<ISource, SerialPortMTSource>();
                        break;
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
                        services.AddSingleton(ctx => SerialPortConnection.FromMock<SerialPortFGMock>(ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
                        break;
                    default:
                        services.AddSingleton(ctx => SerialPortConnection.FromMock<SerialPortMTMock>(ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
                        break;
                }
            else
            {
                var portName = configuration["SerialPort:PortName"];

                if (string.IsNullOrEmpty(portName))
                    throw new NotSupportedException("serial port name must be set if not using serial port mock.");

                if (portName.Contains(':'))
                    services.AddSingleton(ctx => SerialPortConnection.FromNetwork(portName, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
                else
                    services.AddSingleton(ctx => SerialPortConnection.FromSerialPort(portName, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
            }

            switch (deviceType)
            {
                case "FG":
                    services.AddSingleton<IRefMeter, SerialPortFGRefMeter>();
                    services.AddSingleton<IErrorCalculator, SerialPortFGErrorCalculator>();
                    services.AddSingleton<IFrequencyGenerator, SerialPortFGFrequencyGenerator>();
                    break;
                default:
                    services.AddSingleton<IRefMeter, SerialPortMTRefMeter>();
                    services.AddSingleton<IErrorCalculator, SerialPortMTErrorCalculator>();
                    services.AddSingleton<IFrequencyGenerator, SerialPortMTFrequencyGenerator>();
                    break;
            }
        }
    }
}
