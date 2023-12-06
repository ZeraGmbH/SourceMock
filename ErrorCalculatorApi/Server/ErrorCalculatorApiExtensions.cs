using ErrorCalculatorApi.Actions.Device;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ErrorCalculatorApi;

/// <summary>
/// 
/// </summary>
public static class ErrorCalculatorApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseErrorCalculatorApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ErrorCalculatorApiConfiguration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseErrorCalculatorApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseErrorCalculatorApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ISerialPortFGErrorCalculator, SerialPortFGErrorCalculator>();
        services.AddTransient<ISerialPortMTErrorCalculator, SerialPortMTErrorCalculator>();
    }
}
