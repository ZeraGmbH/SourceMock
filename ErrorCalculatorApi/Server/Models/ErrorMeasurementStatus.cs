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
    /// Current energy in W - if known.
    /// </summary>
    public double? Energy { get; set; }

    /// <summary>
    /// Energy from the reference meter - either as wnergy (Wh)
    /// or as a number of impulses.
    /// </summary>
    public double? ReferenceCountsOrEnergy { get; set; }

    /// <summary>
    /// Energy seen by the device under test - either as wnergy (Wh)
    /// or as a number of impulses.
    /// </summary>
    public double? MeterCountsOrEnergy { get; set; }

    /// <summary>
    /// Set if the counts are in energy, unset for imulses.
    /// </summary>
    public bool? CountsAreEnergy { get; set; }
}