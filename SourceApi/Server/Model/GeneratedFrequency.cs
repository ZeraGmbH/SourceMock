using ZERA.WebSam.Shared.DomainSpecific;

namespace SourceApi.Model;

/// <summary>
/// Describes how the frequency should be generated.
/// </summary>
[Serializable]
public class GeneratedFrequency
{
    /// <summary>
    /// The mode of how the frequency should be generated
    /// </summary>
    public FrequencyMode Mode { get; set; }

    /// <summary>
    /// If applicable, the value of the frequency in Hertz.
    /// </summary>
    public Frequency Value { get; set; }
}
