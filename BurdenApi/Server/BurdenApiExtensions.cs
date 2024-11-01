using BurdenApi.Actions;
using BurdenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
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

        services.AddKeyedSingleton("Burden", (ctx, key) =>
        {
            if (!"MeterTestSystem".Equals(key)) throw new ArgumentException("wrong service key", nameof(key));

            return ctx.GetRequiredService<BurdenFactory>().Connection;
        });
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