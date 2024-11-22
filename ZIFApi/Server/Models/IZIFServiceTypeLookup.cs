namespace ZIFApi.Models;

/// <summary>
/// Helper to retrieve service types from a meter form.
/// </summary>
public interface IZIFServiceTypeLookup
{
    /// <summary>
    /// Lookup service types.
    /// </summary>
    /// <param name="meterForm">The corresping meter form.</param>
    /// <returns>All service types of the meter form.</returns>
    Task<string[]> GetServiceTypesOfMeterForm(string meterForm);
}