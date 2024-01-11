using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi;

/// <summary>
/// </summary>
public class ErrorConditions
{
    /// <summary>
    /// Individual error conditions for all amplifiers.
    /// </summary>
    [NotNull, Required]
    public Dictionary<Amplifiers, AmplifierErrorConditions> Amplifiers { get; set; } = [];
}
