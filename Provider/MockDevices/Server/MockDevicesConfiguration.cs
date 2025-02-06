using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockDevices.ErrorCalculator;
using MockDevices.MeterTestSystem;
using MockDevices.ReferenceMeter;
using MockDevices.Source;
using Swashbuckle.AspNetCore.SwaggerGen;
using ZERA.WebSam.Shared.Provider;

namespace MockDevices;

/// <summary>
/// 
/// </summary>
public static class MockDevicesConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public static void UseMockDevices(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(MockDevicesConfiguration).Assembly.GetName().Name}.xml"), true);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseMockDevices(this IEndpointRouteBuilder app)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void UseMockDevices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IErrorCalculatorMock, ErrorCalculatorMock>();

        services.AddTransient<IACSourceMock, ACSourceMock>();
        services.AddTransient<IMockRefMeter, ACRefMeterMock>();
        services.AddTransient<MeterTestSystemAcMock>();

        services.AddTransient<IDCSourceMock, DCSourceMock>();
        services.AddTransient<IDCRefMeterMock, DCRefMeterMock>();
        services.AddTransient<MeterTestSystemDcMock>();
    }
}