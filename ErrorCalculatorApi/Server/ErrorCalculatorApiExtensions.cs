using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Actions.Device.MAD;
using ErrorCalculatorApi.Exceptions;
using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared;
using Swashbuckle.AspNetCore.SwaggerGen;
using ErrorCalculatorApi.Actions;

namespace ErrorCalculatorApi;

/// <summary>
/// Error calculator API configuration if used in a web server.
/// </summary>
public static class ErrorCalculatorApiConfiguration
{
    /// <summary>
    /// Configure OpenAPI configuration.
    /// </summary>
    /// <param name="options">Documentation builder instance.</param>
    public static void UseErrorCalculatorApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ErrorCalculatorApiConfiguration).Assembly.GetName().Name}.xml"), true);

        SwaggerModelExtender.AddType<ErrorCalculatorApiErrorCodes>().Register(options);
    }

    /// <summary>
    /// Add ErrorCalculatorExceptionFilter to local scope
    /// </summary>
    public static void UseErrorCalculatorApi(this MvcOptions options)
    {
        options.Filters.Add<ErrorCalculatorExceptionFilter>();
    }

    /// <summary>
    /// Provide additional routing configuration - e.g. for SignalR.
    /// </summary>
    /// <param name="app">Routing configurator.</param>
    public static void UseErrorCalculatorApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// Configure dependencyy injection.
    /// </summary>
    /// <param name="services">Dependency injection builder.</param>
    /// <param name="configuration">Current web server configuration.</param>
    public static void UseErrorCalculatorApi(this IServiceCollection services, IConfiguration configuration)
    {
        /* Make all implementations transient since lifetime is controlled by a meter test system. */
        services.AddTransient<IErrorCalculatorMock, ErrorCalculatorMock>();
        services.AddTransient<ISerialPortFGErrorCalculator, SerialPortFGErrorCalculator>();
        services.AddTransient<ISerialPortMTErrorCalculator, SerialPortMTErrorCalculator>();

        services.AddTransient<IErrorCalculatorFactory, ErrorCalculatorFactory>();
        services.AddKeyedTransient<IErrorCalculatorInternal, Mad1ErrorCalculator>(ErrorCalculatorProtocols.MAD_1);
        services.AddKeyedTransient<IErrorCalculatorInternal, RestErrorCalculator>(ErrorCalculatorProtocols.HTTP);

        services.AddKeyedTransient<IMadConnection, MadTcpConnection>(ErrorCalculatorConnectionTypes.TCP);
    }
}
