using Microsoft.OpenApi.Models;
using SharedLibrary.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedLibrary.ExceptionHandling;

/// <summary>
/// 
/// </summary>
public class ErrorSchemas : IDocumentFilter
{
    private static void Apply<T>(DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="swaggerDoc"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        Apply<SamDetailExtensions>(context);
    }

}