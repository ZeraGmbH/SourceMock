using DeviceApiLib.Actions.Database;

using DeviceApiSharedLibrary.Actions.Database;
using DeviceApiSharedLibrary.Models;
using DeviceApiSharedLibrary.Services;

using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using RefMeterApi.Services;

using SerialPortProxy;

using Swashbuckle.AspNetCore.SwaggerGen;

using WebSamDeviceApis.Actions.Device;
using WebSamDeviceApis.Actions.SerialPort;
using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Actions.VeinSource;

namespace WebSamDeviceApis;

public static class Configuration
{
    public static void UseDeviceApi(this SwaggerGenOptions options)
    {
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
            var portName = configuration["SerialPort:PortName"];
            var mockType = configuration["SerialPort:PortMockType"];

            if (!string.IsNullOrEmpty(portName))
            {
                if (!string.IsNullOrEmpty(mockType))
                    throw new NotSupportedException("serial port name and port mock type must not be both set.");

                services.AddSingleton(ctx => SerialPortConnection.FromSerialPort(portName, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
            }
            else if (!string.IsNullOrEmpty(mockType))
            {
                services.AddSingleton(ctx => SerialPortConnection.FromMock(Type.GetType(mockType)!, ctx.GetRequiredService<ILogger<SerialPortConnection>>()));
            }
            else
                throw new NotSupportedException("either serial port name or port mock type must be set.");

            services.AddSingleton<IRefMeterDevice, SerialPortRefMeterDevice>();
            services.AddSingleton<ISourceDevice, SerialPortSourceDevice>();
        }

        {
            var mongoDb = configuration.GetSection("MongoDB").Get<MongoDbSettings>();

            if (!string.IsNullOrEmpty(mongoDb?.ServerName) && !string.IsNullOrEmpty(mongoDb?.DatabaseName))
            {
                services.AddSingleton(mongoDb);
                services.AddSingleton<IMongoDbDatabaseService, MongoDbDatabaseService>();
                services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(MongoDbCollectionFactory<>));
                services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(MongoDbHistoryCollectionFactory<>));
            }
            else
            {
                services.AddTransient(typeof(IObjectCollectionFactory<>), typeof(InMemoryCollectionFactory<>));
                services.AddTransient(typeof(IHistoryCollectionFactory<>), typeof(InMemoryHistoryCollectionFactory<>));
            }

            services.AddSingleton<IDeviceUnderTestStorage, DeviceUnderTestStorage>();
        }
    }
}
