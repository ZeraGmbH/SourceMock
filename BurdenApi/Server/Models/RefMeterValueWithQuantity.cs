using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Measurement value with additional power information.
/// </summary>
public class RefMeterValueWithQuantity : GoalValueWithQuantity
{
    /// <summary>
    /// Full data of the measured phase.
    /// </summary>
    [NotNull, Required]
    public MeasuredLoadpointPhase Phase { get; set; } = new();

    /// <summary>
    /// Meaured frequency.
    /// </summary>
    [NotNull, Required]
    public Frequency? Frequency { get; set; }

    /// <summary>
    /// Measured voltage range.
    /// </summary>
    [NotNull, Required]
    public Voltage? VoltageRange { get; set; }

    /// <summary>
    /// Measrued current range.
    /// </summary>
    [NotNull, Required]
    public Current? CurrentRange { get; set; }
}
