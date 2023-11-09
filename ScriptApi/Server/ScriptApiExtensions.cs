using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ScriptApi.Controllers;
using ScriptApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScriptApi;

/// <summary>
/// 
/// </summary>
public static class Configuration
{
    class SignalRExtraSchemas : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(ScriptEngineVersion), context.SchemaRepository);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public static void UseScriptApi(this SwaggerGenOptions options)
    {
        options.DocumentFilter<SignalRExtraSchemas>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public static void UseScriptApi(this IEndpointRouteBuilder app)
    {
        /* Register all SignalR (Web Socket) servers provided by the DeviceApi. */
        app.MapHub<ScriptEngineHub>("/api/v1/ScriptEngine");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void UseScriptApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Use SignalR (Web Sockets).
        services.AddSignalR();
    }
}
