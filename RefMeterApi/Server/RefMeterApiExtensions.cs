using RefMeterApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RefMeterApi;

/// <summary>
/// 
/// </summary>
public static class RefMeterApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseRefMeterApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(RefMeterApiConfiguration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseRefMeterApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseRefMeterApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ISerialPortFGRefMeter, SerialPortFGRefMeter>();
        services.AddTransient<ISerialPortMTRefMeter, SerialPortMTRefMeter>();
    }
}
