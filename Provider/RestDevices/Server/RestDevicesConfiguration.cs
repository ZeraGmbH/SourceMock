using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockDevices.ErrorCalculator;
using MockDevices.ReferenceMeter;
using MockDevices.Source;
using RestDevices.Controller;
using RestDevices.Dosage;
using RestDevices.ErrorCalculator;
using RestDevices.ReferenceMeter;
using RestDevices.Source;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZERA.WebSam.Shared.Provider;

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

        switch (configuration.GetValue<string>("UseSourceRestMock"))
        {

            case "AC":
                services.AddKeyedSingleton<ISource, ACSourceMock>(SourceRestMockController.MockKey);
                break;
            case "DC":
                services.AddKeyedSingleton<ISource, DCSourceMock>(SourceRestMockController.MockKey);
                break;
            default:
                services.AddKeyedSingleton<ISource, UnavailableSource>(SourceRestMockController.MockKey);
                break;
        }

        switch (configuration.GetValue<string>("UseReferenceMeterRestMock"))
        {
            case "AC":
                services.AddKeyedSingleton<IRefMeter, ACRefMeterMock>(RefMeterRestMockController.MockKey);
                break;
            case "DC":
                services.AddKeyedSingleton<IRefMeter, DCRefMeterMock>(RefMeterRestMockController.MockKey);
                break;
            default:
                services.AddKeyedSingleton<IRefMeter, UnavailableReferenceMeter>(RefMeterRestMockController.MockKey);
                break;
        }

        switch (configuration.GetValue<bool>("UseErrorCalculatorRestMock"))
        {
            case true:
                services.AddKeyedSingleton<IErrorCalculator, ErrorCalculatorMock>(ErrorCalculatorRestMockController.MockKey);
                break;
            default:
                services.AddKeyedSingleton<IErrorCalculator, UnavailableErrorCalculator>(ErrorCalculatorRestMockController.MockKey);
                break; ;
        }

        services.AddKeyedTransient<IDosage>(DosageRestMockController.MockKey, (ctx, key) => ctx.GetRequiredKeyedService<ISource>(SourceRestMockController.MockKey));
    }
}