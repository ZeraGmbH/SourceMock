using BarcodeApi.Actions;
using BarcodeApi.Actions.Device;
using BarcodeApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BarcodeApi;

/// <summary>
/// Barcode API configuration if used in a web server.
/// </summary>
public static class BarcodeApiConfiguration
{
    /// <summary>
    /// Configure OpenAPI configuration.
    /// </summary>
    /// <param name="options">Documentation builder instance.</param>
    public static void UseBarcodeApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(BarcodeApiConfiguration).Assembly.GetName().Name}.xml"), true);
    }

    /// <summary>
    /// Configure dependencyy injection.
    /// </summary>
    /// <param name="services">Dependency injection builder.</param>
    /// <param name="configuration">Current web server configuration.</param>
    public static void UseBarcodeApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IInputDeviceManager, InputDeviceManager>();

        services.AddTransient<BarcodeReaderMock, BarcodeReaderMock>();

        services.AddSingleton<IBarcodeReaderFactory, BarcodeReaderFactory>();

        /* Access barcode reader singleton through the factory. */
        services.AddSingleton((ctx) => ctx.GetRequiredService<IBarcodeReaderFactory>().BarcodeReader);
    }
}
