using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public static class DomainSpecificNumber
{
    /// <summary>
    /// All supported domain specific numbers.
    /// </summary>
    public static readonly ReadOnlyCollection<Type> All =
        typeof(DomainSpecificNumber)
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsValueType && t.IsAssignableTo(typeof(IInternalDomainSpecificNumber)))
            .ToArray()
            .AsReadOnly();

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
            if (context.Type.IsAssignableTo(typeof(IInternalDomainSpecificNumber)))
                schema.Type = "number";
        }
    }

    /// <summary>
    /// Support to convert domain specific numbers from and to JSON as pure numbers.
    /// </summary>
    internal class Converter : JsonConverter<IInternalDomainSpecificNumber>
    {
        /// <summary>
        /// Test if a specific data type represents a domain specific number.
        /// </summary>
        /// <param name="typeToConvert">Type to test.</param>
        /// <returns>Set if the type is a domain specific number.</returns>
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(IInternalDomainSpecificNumber));

        /// <summary>
        /// Read a JSON number and make it a domain specific number.
        /// </summary>
        /// <param name="reader">Raw JSON stream.</param>
        /// <param name="typeToConvert">Type of the number to use.</param>
        /// <param name="options">Serialisation options.</param>
        /// <returns>The reconstructed number.</returns>
        public override IInternalDomainSpecificNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : (IInternalDomainSpecificNumber)Activator.CreateInstance(typeToConvert, reader.GetDouble())!;

        /// <summary>
        /// Serialize a domain specific number to a pure JSON number.
        /// </summary>
        /// <param name="writer">Output JSON stream.</param>
        /// <param name="data">The number to serialize.</param>
        /// <param name="options">Serialization options.</param>
        public override void Write(Utf8JsonWriter writer, IInternalDomainSpecificNumber data, JsonSerializerOptions options)
            => writer.WriteNumberValue(GetValue(data));
    }

    /// <summary>
    /// Helper to serialize a domain specific number.
    /// </summary>
    public class BsonProvider : IBsonSerializationProvider
    {
        /// <summary>
        /// Serialize a domain spcific number as a double.
        /// </summary>
        private class BsonSerializer<T> : SerializerBase<T> where T : struct, IInternalDomainSpecificNumber
        {
            /// <inheritdoc/>
            public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
                => (T)Activator.CreateInstance(typeof(T), context.Reader.ReadDouble())!;

            /// <inheritdoc/>
            public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T data)
                => context.Writer.WriteDouble(GetValue(data));
        }

        /// <inheritdoc/>
        public IBsonSerializer? GetSerializer(Type type)
            => type.IsAssignableTo(typeof(IInternalDomainSpecificNumber))
                ? (IBsonSerializer?)Activator.CreateInstance(typeof(BsonSerializer<>).MakeGenericType(type))
                : null;
    }

    /// <summary>
    /// Get the current value from a domain specific number.
    /// </summary>
    /// <param name="data">Some domain specific number.</param>
    /// <returns>The value as a double.</returns>
    private static double GetValue(IInternalDomainSpecificNumber data)
    {
        /* Read the hidden valiue. */
        var value = data.GetValue();

        /* Disallow bad numbers - we ignore options here! */
        if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentException("no JSON representation for given number");

        return value;
    }
}