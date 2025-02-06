using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator.FG30x;
using ZeraDevices.ReferenceMeter.MeterConstantCalculator.MT768;
using ZeraDevices.Source;
using ZeraDevices.Source.FG30x;
using ZeraDevices.Source.MT768;

namespace ZeraDevices;

/// <summary>
/// 
/// </summary>
public static class ZeraDevicesConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseZeraDevices(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ZeraDevicesConfiguration).Assembly.GetName().Name}.xml"), true);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseZeraDevices(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseZeraDevices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMeterConstantCalculator, MeterConstantCalculator>();

        services.AddSingleton<ICapabilitiesMap, CapabilitiesMap>();

        services.AddTransient<ISerialPortFGSource, SerialPortFGSource>();
        services.AddTransient<ISerialPortFGRefMeter, SerialPortFGRefMeter>();

        services.AddTransient<ISerialPortMTSource, SerialPortMTSource>();
        services.AddTransient<ISerialPortMTRefMeter, SerialPortMTRefMeter>();

        services.AddKeyedSingleton<ISerialPortConnectionFactory, SerialPortConnectionFactory>("MeterTestSystem");
    }
}