using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Measurement value with additional range quantity.
/// </summary>
public class GoalValueWithQuantity : GoalValue
{
    /// <summary>
    /// The measured quantity.
    /// </summary>
    public double? Range { get; set; }

}
