using BurdenApi.Actions.Algorithms;
using BurdenApi.Actions.Device;
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

        services.AddKeyedTransient<ICalibrationAlgorithm, IntervalCalibrator>(CalibrationAlgorithms.Interval);
        services.AddKeyedTransient<ICalibrationAlgorithm, SingleStepCalibrator>(CalibrationAlgorithms.SingleStep);

        services.AddKeyedTransient<ICalibrationAlgorithm, SingleStepCalibrator>(CalibrationAlgorithms.Default);

        services.AddScoped<ICalibrator, Calibrator>();

        services.AddKeyedSingleton("Burden", (ctx, key) => ctx.GetRequiredService<IBurdenFactory>().Connection);

        services.AddScoped<CalibrationHardwareMock>();
        services.AddScoped<CalibrationHardware>();

        services.AddScoped(ctx => ctx.GetRequiredService<IBurdenFactory>().CreateHardware(ctx));
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