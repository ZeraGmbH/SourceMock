using System.Text.Json;
using System.Text.Json.Nodes;

namespace SharedLibrary;

/// <summary>
/// 
/// </summary>
public static class LibUtils
{
    /// <summary>
    /// Configure serializer to generate camel casing.
    /// </summary>
    public static readonly JsonSerializerOptions JsonSettings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Copys values of object, not its reference
    /// </summary>
    /// <typeparam name="T">Any type of object</typeparam>
    /// <param name="self">object to clone</param>
    /// <returns>a clone of the object</returns>
    public static T DeepCopyAs<T>(object? self) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(self, JsonSettings), JsonSettings)!;

    /// <summary>
    /// Copys values of object, not its reference
    /// </summary>
    /// <typeparam name="T">Any type of object</typeparam>
    /// <param name="self">object to clone</param>
    /// <returns>a clone of the object</returns>
    public static T DeepCopy<T>(T self) => DeepCopyAs<T>(self);

    /// <summary>
    /// Extract a scalar value from a JsonElement.
    /// </summary>
    /// <param name="json">As parsed with the System.Text.Json library.</param>
    /// <returns>The value.</returns>
    public static object? ToJsonScalar(this JsonElement json)
        => json.ValueKind switch
        {
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Number => json.Deserialize<double>(),
            JsonValueKind.String => json.Deserialize<string>(),
            JsonValueKind.True => true,
            _ => json,
        };

    /// <summary>
    /// Deserialize a JSON element to an instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="node">Element to deserialize.</param>
    /// <returns>Instance - may be null if JSON element represents null.</returns>
    public static T? DefaultDeserialize<T>(this JsonNode node) => JsonSerializer.Deserialize<T>(node, JsonSettings);

    /// <summary>
    /// Deserialize a JSON element to an instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="element">Element to deserialize.</param>
    /// <returns>Instance - may be null if JSON element represents null.</returns>
    public static T? DefaultDeserialize<T>(this JsonElement element) => JsonSerializer.Deserialize<T>(element, JsonSettings);
}