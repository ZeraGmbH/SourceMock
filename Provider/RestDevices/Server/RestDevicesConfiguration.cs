using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        var sourceRestMock = configuration.GetValue<string>("UseSourceRestMock");

        if (sourceRestMock == "AC")
            services.AddKeyedSingleton<ISource, ACSourceMock>(SourceRestMockController.MockKey);
        else if (sourceRestMock == "DC")
            services.AddKeyedSingleton<ISource, DCSourceMock>(SourceRestMockController.MockKey);
        else
            services.AddKeyedSingleton<ISource, UnavailableSource>(SourceRestMockController.MockKey);

        var refMeterRestMock = configuration.GetValue<string>("UseReferenceMeterRestMock");

        if (refMeterRestMock == "AC")
            services.AddKeyedSingleton<IRefMeter, ACRefMeterMock>(RefMeterRestMockController.MockKey);
        else if (refMeterRestMock == "DC")
            services.AddKeyedSingleton<IRefMeter, DCRefMeterMock>(RefMeterRestMockController.MockKey);
        else
            services.AddKeyedSingleton<IRefMeter, UnavailableReferenceMeter>(RefMeterRestMockController.MockKey);

        services.AddKeyedTransient<IDosage>(DosageRestMockController.MockKey, (ctx, key) => ctx.GetRequiredKeyedService<ISource>(SourceRestMockController.MockKey));
    }
}