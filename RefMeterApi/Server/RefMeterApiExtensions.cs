using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Actions.Device;
using RefMeterApi.Actions.MeterConstantCalculator;
using RefMeterApi.Actions.RestSource;
using RefMeterApi.Exceptions;
using ZERA.WebSam.Shared;
using Swashbuckle.AspNetCore.SwaggerGen;
using RefMeterApi.Controllers;
using ZERA.WebSam.Shared.Provider;
using ZERA.WebSam.Shared.Models.ReferenceMeter;

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
        services.AddTransient<IMeterConstantCalculator, MeterConstantCalculator>();

        /* The concrete implementations are transient and the lifetime is controlled by the corresponding meter test system. */
        services.AddTransient<IMockRefMeter, ACRefMeterMock>();
        services.AddTransient<IDCRefMeterMock, DCRefMeterMock>();
        services.AddTransient<IRestRefMeter, RestRefMeter>();
        services.AddTransient<ISerialPortFGRefMeter, SerialPortFGRefMeter>();
        services.AddTransient<ISerialPortMTRefMeter, SerialPortMTRefMeter>();

        var restMock = configuration.GetValue<string>("UseReferenceMeterRestMock");

        if (restMock == "AC")
            services.AddKeyedSingleton<IRefMeter, ACRefMeterMock>(RefMeterRestMockController.MockKey);
        else if (restMock == "DC")
            services.AddKeyedSingleton<IRefMeter, DCRefMeterMock>(RefMeterRestMockController.MockKey);
        else
            services.AddKeyedSingleton<IRefMeter, UnavailableReferenceMeter>(RefMeterRestMockController.MockKey);
    }

    /// <summary>
    /// Add RefMeterApiExceptionFilter to local scope
    /// </summary>
    public static void UseRefMeterApi(this MvcOptions options)
    {
        options.Filters.Add<RefMeterApiExceptionFilter>();
    }
}
