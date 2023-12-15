using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedLibrary;

/// <summary>
/// Helper class to add models to the OpenAPI documentation which
/// are not referenced otherwise.
/// </summary>
public class SwaggerModelExtender(Type[] types) : IDocumentFilter
{
    /// <summary>
    /// Fluent interface to collect types.
    /// </summary>
    public interface ITypeArrayBuilder
    {
        /// <summary>
        /// Add a model type to the collection.
        /// </summary>
        /// <typeparam name="T">Type to add.</typeparam>
        /// <returns>Current type collector.</returns>
        ITypeArrayBuilder AddType<T>();

        /// <summary>
        /// Register all collected types in OpenAPI.
        /// </summary>
        /// <param name="options">Swagger OpenID context.</param>
        void Register(SwaggerGenOptions options);
    }

    /// <summary>
    /// Type collector class.
    /// </summary>
    class TypeArrayBuilder : ITypeArrayBuilder
    {
        /// <summary>
        /// All types collected so far.
        /// </summary>
        private readonly List<Type> _types = [];

        /// <inheritdoc/>
        public ITypeArrayBuilder AddType<T>()
        {
            _types.Add(typeof(T));

            return this;
        }

        /// <inheritdoc/>
        public void Register(SwaggerGenOptions options) =>
            options.DocumentFilter<SwaggerModelExtender>([_types.ToArray()]);
    }

    /// <summary>
    /// List of types to register.
    /// </summary>
    private readonly Type[] _types = types;

    /// <summary>
    /// Create a brand new type collector.
    /// </summary>
    /// <typeparam name="T">First type to add to the collector.</typeparam>
    /// <returns>Fluent type collector interface.</returns>
    public static ITypeArrayBuilder AddType<T>() => new TypeArrayBuilder().AddType<T>();

    /// <summary>
    /// Register all types in the OpenAPI documentation.
    /// </summary>
    /// <param name="swaggerDoc">Swagger OpenID document context.</param>
    /// <param name="context">Operation context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var type in _types)
            context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
    }
}

