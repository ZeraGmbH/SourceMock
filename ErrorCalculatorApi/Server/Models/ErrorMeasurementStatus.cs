using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

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
    public Impulses? ReferenceCounts { get; set; }

    /// <summary>
    /// Number of meter impulses for the last error measurement.
    /// </summary>
    public Impulses? MeterCounts { get; set; }
}