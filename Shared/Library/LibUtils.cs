using System.Security.Claims;
using System.Text.Json;

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
    /// Retrieve the unique identifier of a user.
    /// </summary>
    /// <param name="user">User Information as provided by the runtime.</param>
    /// <returns>User identification.</returns>
    public static string? GetUserId(ClaimsPrincipal? user = null) =>
        user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

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
}