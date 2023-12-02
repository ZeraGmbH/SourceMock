
using ErrorCalculatorApi;

using FrequencyGeneratorApi;

using RefMeterApi;

using SourceApi;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebSamDeviceApis;

public static class Configuration
{
    public static void UseDeviceApi(this SwaggerGenOptions options)
    {
        options.UseErrorCalculatorApi();
        options.UseFrequencyGeneratorApi();
        options.UseRefMeterApi();
        options.UseSourceApi();
    }

    public static void UseDeviceApi(this IEndpointRouteBuilder app)
    {
        app.UseErrorCalculatorApi();
        app.UseFrequencyGeneratorApi();
        app.UseRefMeterApi();
        app.UseSourceApi();
    }

    public static void UseDeviceApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.UseErrorCalculatorApi(configuration);
        services.UseFrequencyGeneratorApi(configuration);
        services.UseRefMeterApi(configuration);
        services.UseSourceApi(configuration);
    }
}
