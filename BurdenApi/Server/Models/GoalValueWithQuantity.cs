using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Measurement value with additional range quantity.
/// </summary>
public class GoalValueWithQuantity(ApparentPower power, PowerFactor factor, double? range) : GoalValue(power, factor)
{
    /// <summary>
    /// The measured quantity.
    /// </summary>
    public double? Range { get; set; } = range;

    /// <summary>
    /// To support serialisation.
    /// </summary>
    public GoalValueWithQuantity() : this(new(0), new(1), null) { }
}
