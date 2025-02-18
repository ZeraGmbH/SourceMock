using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.Provider;
using ZeraDevices.ErrorCalculator.STM;
using ZeraDevices.ErrorCalculator.STM.Exceptions;
using ZeraDevices.MeterTestSystem.FG30x;
using ZeraDevices.MeterTestSystem.MT768;
using ZeraDevices.ReferenceMeter.ErrorCalculator.MT768;
using ZeraDevices.ReferenceMeter.FG30x;
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
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(PhysicalPortProxy).Assembly.GetName().Name}.xml"), true);
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
    /// <param name="options"></param>
    public static void UseZeraDevices(this MvcOptions options)
    {
        options.Filters.Add<ErrorCalculatorExceptionFilter>();
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
        services.AddTransient<SerialPortFGMeterTestSystem>();

        services.AddTransient<ISerialPortMTSource, SerialPortMTSource>();
        services.AddTransient<ISerialPortMTRefMeter, SerialPortMTRefMeter>();
        services.AddTransient<ISerialPortMTErrorCalculator, SerialPortMTErrorCalculator>();
        services.AddTransient<SerialPortMTMeterTestSystem>();

        services.AddKeyedTransient<IErrorCalculatorSetup, Mad1ErrorCalculator>(ErrorCalculatorProtocols.MAD_1);
        services.AddKeyedTransient<IMadConnection, MadTcpConnection>(ErrorCalculatorConnectionTypes.TCP);

        services.AddKeyedSingleton<ISerialPortConnectionFactory, SerialPortConnectionFactory>("ZERASerial");

        services.AddKeyedTransient(KeyedService.AnyKey, (ctx, key) => ctx.GetRequiredKeyedService<ISerialPortConnectionFactory>(key).Connection);
    }
}