using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Exceptions;
using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(ErrorCalculatorApiConfiguration).Assembly.GetName().Name}.xml"));

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
        services.AddTransient<ISerialPortFGErrorCalculator, SerialPortFGErrorCalculator>();
        services.AddTransient<ISerialPortMTErrorCalculator, SerialPortMTErrorCalculator>();
    }
}
