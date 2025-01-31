namespace BarcodeApi.Actions;

/// <summary>
/// Describe a single system input device.
/// /// </summary>
public interface IInputDevice
{
    /// <summary>
    /// Report a single string propery.
    /// </summary>
    /// <param name="key">Name of the property.</param>
    /// <returns>String value or null if unknown.</returns>
    string? GetProperty(string key);

    /// <summary>
    /// Report a list of values.
    /// </summary>
    /// <param name="key">Name of the list.</param>
    /// <returns>List of string values.</returns>
    HashSet<string>? GetList(string key);

    /// <summary>
    /// Report a bit mask of unrestricted length.
    /// </summary>
    /// <param name="key">Name of the mask.</param>
    /// <returns>The full mask in LSB order.</returns>
    ulong[]? GetBits(string key);
}
