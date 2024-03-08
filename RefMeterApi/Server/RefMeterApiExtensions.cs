using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Actions.Device;
using RefMeterApi.Actions.RestSource;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;
using SharedLibrary;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RefMeterApi;

/// <summary>
/// Confuguration for a web server using the reference meter API.
/// </summary>
public static class RefMeterApiConfiguration
{
    /// <summary>
    /// Configure the OpenAP documentation.
    /// </summary>
    /// <param name="options">Documentation builder.</param>
    public static void UseRefMeterApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(RefMeterApiConfiguration).Assembly.GetName().Name}.xml"), true);

        SwaggerModelExtender.AddType<RefMeterApiErrorCodes>().Register(options);
    }

    /// <summary>
    /// Configure additional routes.
    /// </summary>
    /// <param name="app">Routing builder.</param>
    public static void UseRefMeterApi(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// Configure the dependency injection system.
    /// </summary>
    /// <param name="services">Dependency injection builder.</param>
    /// <param name="configuration">Configuration of the web server.</param>
    public static void UseRefMeterApi(this IServiceCollection services, IConfiguration configuration)
    {
        /* The concrete implementations are transient and the lifetime is controlled by the corresponding meter test system. */
        services.AddTransient<IMockRefMeter, RefMeterMock>();
        services.AddTransient<IRestRefMeter, RestRefMeter>();
        services.AddTransient<ISerialPortFGRefMeter, SerialPortFGRefMeter>();
        services.AddTransient<ISerialPortMTRefMeter, SerialPortMTRefMeter>();
    }

    /// <summary>
    /// Add RefMeterApiExceptionFilter to local scope
    /// </summary>
    public static void UseRefMeterApi(this MvcOptions options)
    {
        options.Filters.Add<RefMeterApiExceptionFilter>();
    }
}
