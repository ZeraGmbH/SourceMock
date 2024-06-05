using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// Current status of the error measurement.
/// </summary>
public class ErrorMeasurementStatus
{
    /// <summary>
    /// Current state of the operation.
    /// </summary>
    [NotNull, Required]
    public ErrorMeasurementStates State { get; set; }

    /// <summary>
    /// Error result of the last completed measurement - if available.
    /// </summary>
    public double? ErrorValue { get; set; }

    /// <summary>
    /// Current progress in % - if available.
    /// </summary>
    public double? Progress { get; set; }

    /// <summary>
    /// Energy from the reference meter - either as energy (Wh)
    /// or as a number of impulses.
    /// </summary>
    public double? ReferenceCountsOrEnergy { get; set; }

    /// <summary>
    /// Energy seen by the device under test - either as energy (Wh)
    /// or as a number of impulses.
    /// </summary>
    public double? MeterCountsOrEnergy { get; set; }

    /// <summary>
    /// Set if the counts are in energy, unset for impulses.
    /// </summary>
    public bool? CountsAreEnergy { get; set; }
}