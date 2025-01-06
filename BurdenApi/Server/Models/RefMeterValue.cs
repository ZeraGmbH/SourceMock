using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// An extended pair of values.
/// </summary>
public class RefMeterValue(ApparentPower apparentPower, ActivePower activePower, ReactivePower reactivePower, PowerFactor factor) : GoalValue(apparentPower, factor)
{
    /// <summary>
    /// Active power in W.
    /// </summary>
    [NotNull, Required]
    public ActivePower ActivePower { get; set; } = activePower;

    /// <summary>
    /// Reactive power in VAr.
    /// </summary>
    [NotNull, Required]
    public ReactivePower ReactivePower { get; set; } = reactivePower;

    /// <summary>
    /// Create a new values information.
    /// </summary>
    public RefMeterValue() : this(new(0), new(0), new(0), new(1)) { }

    /// <summary>
    /// Silent convert from an internal measurement to the protocol representation.
    /// </summary>
    /// <param name="values">Measured values.</param>
    public static implicit operator RefMeterValue(RefMeterValueWithQuantity values) => new(values.ApparentPower, values.ActivePower, values.ReactivePower, values.PowerFactor);
}