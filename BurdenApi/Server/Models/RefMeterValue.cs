using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// An extended pair of values.
/// </summary>
public class RefMeterValue(ApparentPower apparentPower, Frequency? frequency, MeasuredLoadpointPhase phase, PowerFactor factor) : GoalValue(apparentPower, factor)
{
    /// <summary>
    /// Full data of the measured phase.
    /// </summary>
    [NotNull, Required]
    public MeasuredLoadpointPhase Phase { get; set; } = phase;

    /// <summary>
    /// Meaured frequency.
    /// </summary>
    public Frequency? Frequency { get; set; } = frequency;

    /// <summary>
    /// Create a new values information.
    /// </summary>
    public RefMeterValue() : this(new(0), null, new(), new(1)) { }

    /// <summary>
    /// Silent convert from an internal measurement to the protocol representation.
    /// </summary>
    /// <param name="values">Measured values.</param>
    public static implicit operator RefMeterValue(RefMeterValueWithQuantity values) => new(values.ApparentPower, values.Frequency, values.Phase, values.PowerFactor);
}