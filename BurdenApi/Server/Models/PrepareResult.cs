using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Result of a prepare information.
/// </summary>
public class PrepareResult
{
    /// <summary>
    /// Factor used - may be clipped due to voltage restrictions.
    /// </summary>
    [NotNull, Required]
    public double Factor { get; set; }

    /// <summary>
    /// Primary range choosen.
    /// </summary>
    [NotNull, Required]
    public double Range => IsVoltageNotCurrentBurden ? (double)VoltageRange : (double)CurrentRange;

    /// <summary>
    /// Voltage range enforced - may be different from measured range.
    /// </summary>
    [NotNull, Required]
    public Voltage VoltageRange { get; set; }

    /// <summary>
    /// Current range enforced - may be different from measured range.
    /// </summary>
    [NotNull, Required]
    public Current CurrentRange { get; set; }

    /// <summary>
    /// Set if working on a voltage burden.
    /// </summary>
    [NotNull, Required]
    public bool IsVoltageNotCurrentBurden { get; set; }
}
