using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// A relative target pair of values, all calculated
/// as (ACTUAL - EXPECTED) / EXPECTED.
/// </summary>
public class GoalDeviation(double power, double factor)
{
    /// <summary>
    /// Für die Serialisierung.
    /// </summary>
    public GoalDeviation() : this(0, 0) { }

    /// <summary>
    /// Deviation on apparent power, positive if the 
    /// measure value is too large.
    /// </summary>
    [NotNull, Required]
    public double DeltaPower { get; set; } = power;

    /// <summary>
    /// Deviation on power factor, positive if the 
    /// measure value is too large.
    /// </summary>
    [NotNull, Required]
    public double DeltaFactor { get; set; } = factor;

    /// <inheritdoc/>
    public override string ToString() => $"{100 * DeltaPower}%/{100 * DeltaFactor}%";
}
