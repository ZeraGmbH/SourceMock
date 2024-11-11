using BurdenApi.Actions;
using BurdenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BurdenApi;

/// <summary>
/// 
/// </summary>
public static class BurdenApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void UseBurdenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBurdenFactory, BurdenFactory>();
        services.AddSingleton<IBurden, Burden>();

        services.AddTransient<ICalibrator, Calibrator>();

        services.AddKeyedSingleton("Burden", (ctx, key) => ctx.GetRequiredService<IBurdenFactory>().Connection);

        services.AddTransient<CalibrationHardwareMock>();
        services.AddTransient<CalibrationHardware>();

        services.AddTransient(ctx => ctx.GetRequiredService<IBurdenFactory>().CreateHardware(ctx));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void UseBurdenApi(this SwaggerGenOptions options)
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(BurdenApiConfiguration).Assembly.GetName().Name}.xml"), true);
    }
}