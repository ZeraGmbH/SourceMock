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
    /// Number of reference impulses for the last error measurement.
    /// </summary>
    public long? ReferenceCounts { get; set; }

    /// <summary>
    /// Number of meter impulses for the last error measurement.
    /// </summary>
    public long? MeterCounts { get; set; }
}