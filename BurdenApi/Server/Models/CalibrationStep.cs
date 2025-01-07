using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BurdenApi.Models;

/// <summary>
/// Describes a single iteration step during a calibration.
/// </summary>
public class CalibrationStep
{
    /// <summary>
    /// Current number of iteration.
    /// </summary>
    [NotNull, Required]
    public int Iteration { get; set; }

    /// <summary>
    /// Calibration settings.
    /// </summary>
    [NotNull, Required]
    public Calibration Calibration { get; set; } = null!;

    /// <summary>
    /// See if the current calibration should be kept and 
    /// the next level should be inspected.
    /// </summary>
    [JsonIgnore]
    public bool CalibrationChanged => Calibration != null;

    /// <summary>
    /// The faxtor used to configure the loadpoint.
    /// </summary>
    [NotNull, Required]
    public double Factor => Configuration.Factor;

    /// <summary>
    /// Overall settings for the calibration.
    /// </summary>
    [NotNull, Required]
    public PrepareResult Configuration { get; set; } = new();

    /// <summary>
    /// Actual values for the calibration.
    /// </summary>
    [NotNull, Required]
    public RefMeterValue Values { get; set; } = new();

    /// <summary>
    /// Deviation from the calibration goal.
    /// </summary>
    [NotNull, Required]
    public GoalDeviation Deviation
    {
        get
        {
            return _Deviation;
        }
        set
        {
            _Deviation = value;

            // Set redundant value for sorting results.
            TotalAbsDelta = Math.Abs(value.DeltaPower) + Math.Abs(value.DeltaFactor);
        }
    }

    private GoalDeviation _Deviation = new();

    /// <summary>
    /// Actual values from the burden.
    /// </summary>
    [NotNull, Required]
    public GoalValue BurdenValues { get; set; } = new();

    /// <summary>
    /// Deviation of burden values from the calibration goal.
    /// </summary>
    [NotNull, Required]
    public GoalDeviation BurdenDeviation { get; set; } = new();

    /// <summary>
    /// Sum of the deviation totals as a mean to find better calibrations.
    /// </summary>
    [NotNull, Required]
    public double TotalAbsDelta { get; private set; }

    /// <summary>
    /// Create a description for this step.
    /// </summary>
    public override string ToString() => $"{Calibration}: {Values} ({Deviation})";
}
