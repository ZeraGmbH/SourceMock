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
    /// <param name="voltageNotCurrent">Set if the range set is the voltage.</param>
    /// <returns>Resulting values.</returns>
    Task<RefMeterValueWithQuantity> MeasureAsync(Calibration calibration, bool voltageNotCurrent);

    /// <summary>
    /// Get values from the burden.
    /// </summary>
    /// <returns>Resulting values.</returns>
    Task<GoalValueWithQuantity> MeasureBurdenAsync(bool voltageNotCurrent);

    /// <summary>
    /// Prepare the measurement.
    /// </summary>
    /// <param name="voltageNotCurrent">Set when using a voltage burden.</param>
    /// <param name="frequency">Frequency to use.</param>
    /// <param name="range">Range to use - optional followed by scaling.</param>
    /// <param name="percentage">Percentage of range to use - 1 means take range as is in the loadpoint.</param>
    /// <param name="detectRange">Set to automatically detect the best range from the reference meter.</param>
    /// <param name="power">Target apparent power.</param>
    /// <param name="fixedPercentage">Unset to allow tweaking the percentage to respect the limits of the source.</param>
    /// <returns>Tuple with the percentage of the range really used and the electrical quantity set.</returns>
    Task<PrepareResult> PrepareAsync(bool voltageNotCurrent, string range, double percentage, Frequency frequency, bool detectRange, ApparentPower power, bool fixedPercentage = true);

    /// <summary>
    /// Report the burden associated with this hardware.
    /// </summary>
    IBurden Burden { get; }
}
