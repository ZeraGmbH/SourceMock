namespace BurdenApi.Models;

/// <summary>
/// Describes the version of a burden.
/// </summary>
public class BurdenVersion
{
    /// <summary>
    /// Primary version information - includes model and
    /// type of burden.
    /// </summary>
    public string Version { get; set; } = null!;

    /// <summary>
    /// Supplementary information.
    /// </summary>
    public string Supplement { get; set; } = null!;
}