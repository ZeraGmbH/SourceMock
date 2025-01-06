using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Measurement value with additional power information.
/// </summary>
public class RefMeterValueWithQuantity(ApparentPower apparentPower, ActivePower activePower, ReactivePower reactivePower, PowerFactor factor, double? range)
    : GoalValueWithQuantity(apparentPower, factor, range)
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
    /// To support serialisation.
    /// </summary>
    public RefMeterValueWithQuantity() : this(new(0), new(0), new(0), new(1), null) { }
}
