using SerialPortProxy;

using WebSamDeviceApis.Actions.Device;
using WebSamDeviceApis.Actions.SerialPort;
using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Actions.VeinSource;

namespace WebSamDeviceApis;

public static class Configuration
{
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
            var portName = configuration["SerialPort:PortName"];
            var mockType = configuration["SerialPort:PortMockType"];

            if (!string.IsNullOrEmpty(portName))
            {
                if (!string.IsNullOrEmpty(mockType))
                    throw new NotSupportedException("serial port name and port mock type must not be both set.");

                services.AddSingleton(() => SerialPortConnection.FromSerialPort(portName));
            }
            else if (!string.IsNullOrEmpty(mockType))
            {
                services.AddSingleton(() => SerialPortConnection.FromMock(Type.GetType(mockType)!));
            }
            else
                throw new NotSupportedException("either serial port name or port mock type must be set.");

            services.AddScoped<IDevice, SerialPortDevice>();
        }

    }
}
