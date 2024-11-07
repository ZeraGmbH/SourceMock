using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// A target pair of values.
/// </summary>
public class GoalValue(ApparentPower power, PowerFactor factor)
{
    /// <summary>
    /// To support serialisation.
    /// </summary>
    public GoalValue() : this(new(0), new(1)) { }

    /// <summary>
    /// Apparent power in W.
    /// </summary>
    [NotNull, Required]
    public ApparentPower ApparentPower { get; set; } = power;

    /// <summary>
    /// Powerfactor between as cos of angle difference.
    /// </summary>
    [NotNull, Required]
    public PowerFactor PowerFactor { get; set; } = factor;

    /// <summary>
    /// Calcluate the deviation of a goal value.
    /// </summary>
    /// <param name="actual">Meaured value.</param>
    /// <param name="expected">Expected value.</param>
    /// <returns>Relative deviation.</returns>
    public static GoalDeviation operator /(GoalValue actual, GoalValue expected)
        => new(
            (actual.ApparentPower - expected.ApparentPower) / expected.ApparentPower,
            (actual.PowerFactor - expected.PowerFactor) / expected.PowerFactor);

    /// <inheritdoc/>
    public override string ToString() => $"{ApparentPower}/{PowerFactor}";
}
