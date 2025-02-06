using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestDevices.Dosage;
using RestDevices.ErrorCalculator;
using RestDevices.ReferenceMeter;
using RestDevices.Source;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZeraDevices.ErrorCalculator.STM;

namespace RestDevices;

/// <summary>
/// 
/// </summary>
public static class RestDevicesConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseRestDevices(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(RestDevicesConfiguration).Assembly.GetName().Name}.xml"), true);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseRestDevices(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseRestDevices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IRestDosage, RestDosage>();
        services.AddTransient<IRestSource, RestSource>();
        services.AddTransient<IRestRefMeter, RestRefMeter>();
        services.AddKeyedTransient<IErrorCalculatorSetup, RestErrorCalculator>(ErrorCalculatorProtocols.HTTP);
    }
}