using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Hardware to provide values from the reference meter.
/// </summary>
public interface ICalibrationHardware
{
    /// <summary>
    /// Get new values after setting a calibration.
    /// </summary>
    /// <param name="calibration">Calibration parameters.</param>
    /// <returns>Resulting values.</returns>
    Task<GoalValue> MeasureAsync(Calibration calibration);

    /// <summary>
    /// Get values from the burden.
    /// </summary>
    /// <returns>Resulting values.</returns>
    Task<GoalValue> MeasureBurdenAsync();

    /// <summary>
    /// Prepare the measurement.
    /// </summary>
    /// <param name="frequency">Frequency to use.</param>
    /// <param name="range">Range to use - optional followed by scaling.</param>
    /// <param name="percentage">Percentage of range to use - 1 means take range as is in the loadpoint.</param>
    /// <param name="detectRange">Set to automatically detect the best range from the reference meter.</param>
    /// <param name="power">Target apparent power.</param>
    Task PrepareAsync(string range, double percentage, Frequency frequency, bool detectRange, ApparentPower power);

    /// <summary>
    /// Report the burden associated with this hardware.
    /// </summary>
    IBurden Burden { get; }
}
