using ErrorCalculatorApi.Actions.Device;
using ZERA.WebSam.Shared.Provider;
using ErrorCalculatorApi.Controllers;
using ErrorCalculatorApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using MockDevices.ErrorCalculator;

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
        services.AddTransient<IErrorCalculatorFactory, ErrorCalculatorFactory>();

        if (configuration.GetValue<bool>("UseErrorCalculatorRestMock") == true)
            services.AddKeyedSingleton<IErrorCalculator, ErrorCalculatorMock>(ErrorCalculatorRestMockController.MockKey);
        else
            services.AddKeyedSingleton<IErrorCalculator, UnavailableErrorCalculator>(ErrorCalculatorRestMockController.MockKey);
    }
}
