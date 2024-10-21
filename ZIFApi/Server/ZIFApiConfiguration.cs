using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared;
using ZIFApi.Actions;
using ZIFApi.Models;

namespace ZIFApi;

/// <summary>
/// 
/// </summary>
public static class ZIFApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void UseZIFApi(this IServiceCollection services, IConfiguration configuration)
    {
        // All known ZIF implementations
        services.AddKeyedTransient<IZIFProtocol, PowerMaster8121>(SupportedZIFProtocols.PowerMaster8121);

        // Used to initialize devices from configuration.
        services.AddSingleton<IZIFDevicesFactory, ZIFDevicesFactory>();

        // Register all sockets.
        services.AddTransient(di => di.GetRequiredService<IZIFDevicesFactory>().Devices);
        services.AddTransient(di => di.GetRequiredService<IZIFDevice[]>().FirstOrDefault()!);

        // Register emulations.
        services.AddKeyedTransient<ISerialPort, PowerMaster8121SerialPortMock>(SupportedZIFProtocols.PowerMaster8121);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void UseZIFApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ZIFApiConfiguration).Assembly.GetName().Name}.xml"), true);

        SwaggerModelExtender
         .AddType<ZIFErrorCodes>()
         .Register(options);
    }
}