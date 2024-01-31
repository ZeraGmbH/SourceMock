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
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(BarcodeApiConfiguration).Assembly.GetName().Name}.xml"));
    }

    /// <summary>
    /// Configure dependencyy injection.
    /// </summary>
    /// <param name="services">Dependency injection builder.</param>
    /// <param name="configuration">Current web server configuration.</param>
    public static void UseBarcodeApi(this IServiceCollection services, IConfiguration configuration)
    {
        var device = configuration["BarcodeReader:Device"];

        if (string.IsNullOrEmpty(device))
            services.AddSingleton<IBarcodeReader, BarcodeReaderMock>();
        else
            services.AddSingleton<IBarcodeReader>(ctx => new BarcodeReader(device, ctx.GetRequiredService<ILogger<BarcodeReader>>()));
    }
}
