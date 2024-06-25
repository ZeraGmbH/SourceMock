using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public abstract class DomainSpecificNumber(double value)
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    protected readonly double Value = value;

    /// <summary>
    /// Configure API generator.
    /// </summary>
    public class Filter : ISchemaFilter
    {
        /// <summary>
        /// All domain specific numbers will be pure JSON numbers inside the API.
        /// </summary>
        /// <param name="schema">Schema created so far.</param>
        /// <param name="context">Information on the model processed.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsAssignableTo(typeof(DomainSpecificNumber)))
                schema.Type = "number";
        }
    }

    /// <summary>
    /// Support to convert domain specific numbers from and to JSON as pure numbers.
    /// </summary>
    public class Converter : JsonConverter<DomainSpecificNumber>
    {
        /// <summary>
        /// Test if a specific data type represents a domain specific number.
        /// </summary>
        /// <param name="typeToConvert">Type to test.</param>
        /// <returns>Set if the type is a domain specific number.</returns>
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(DomainSpecificNumber));

        /// <summary>
        /// Read a JSON number and make it a domain specific number.
        /// </summary>
        /// <param name="reader">Raw JSON stream.</param>
        /// <param name="typeToConvert">Type of the number to use.</param>
        /// <param name="options">Serialisation options.</param>
        /// <returns>The reconstructed number.</returns>
        public override DomainSpecificNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : (DomainSpecificNumber)Activator.CreateInstance(typeToConvert, reader.GetDouble())!;

        /// <summary>
        /// Serialize a domain specific number to a pure JSON number.
        /// </summary>
        /// <param name="writer">Output JSON stream.</param>
        /// <param name="data">The number to serialize.</param>
        /// <param name="options">Serialization options.</param>
        public override void Write(Utf8JsonWriter writer, DomainSpecificNumber data, JsonSerializerOptions options)
        {
            /* Read the hidden valiue. */
            var value = (double)data.GetType().GetField(nameof(Value), BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(data)!;

            /* Disallow bad numbers - we ignore options here! */
            if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentException("no JSON representation for given number");

            /* Add to JSON stream. */
            writer.WriteNumberValue(value);
        }
    }
}