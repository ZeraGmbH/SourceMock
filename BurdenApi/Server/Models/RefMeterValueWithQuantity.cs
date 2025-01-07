using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Measurement value with additional power information.
/// </summary>
public class RefMeterValueWithQuantity(ApparentPower apparentPower, Frequency? frequency, MeasuredLoadpointPhase phase, PowerFactor factor, double? range)
    : GoalValueWithQuantity(apparentPower, factor, range)
{
    /// <summary>
    /// Full data of the measured phase.
    /// </summary>
    [NotNull, Required]
    public MeasuredLoadpointPhase Phase { get; set; } = phase;

    /// <summary>
    /// Meaured frequency.
    /// </summary>
    [NotNull, Required]
    public Frequency? Frequency { get; set; } = frequency;

    /// <summary>
    /// To support serialisation.
    /// </summary>
    public RefMeterValueWithQuantity() : this(new(0), null, new(), new(1), null) { }
}
