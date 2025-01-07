using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// A target pair of values.
/// </summary>
public class GoalValue
{
    /// <summary>
    /// Apparent power in VA.
    /// </summary>
    [NotNull, Required]
    public ApparentPower ApparentPower { get; set; }

    /// <summary>
    /// Powerfactor between as cos of angle difference.
    /// </summary>
    [NotNull, Required]
    public PowerFactor PowerFactor { get; set; }

    /// <summary>
    /// Calcluate the deviation of a goal value.
    /// </summary>
    /// <param name="actual">Meaured value.</param>
    /// <param name="expected">Expected value.</param>
    /// <returns>Relative deviation.</returns>
    public static GoalDeviation operator /(GoalValue actual, GoalValue expected)
    {
        var deltaPower = (actual.ApparentPower - expected.ApparentPower) / expected.ApparentPower;
        var deltaFactor = (actual.PowerFactor - expected.PowerFactor) / expected.PowerFactor;

        var actualAngle = Math.Acos((double)actual.PowerFactor);
        var expectedAngle = Math.Acos((double)expected.PowerFactor);

        return new(deltaPower, deltaFactor, 100d * (actualAngle - expectedAngle));
    }

    /// <inheritdoc/>
    public override string ToString() => $"{ApparentPower}/{PowerFactor}";
}
