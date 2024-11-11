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
    /// Set the current loadpoint.
    /// </summary>
    /// <param name="isVoltageNotCurrent">Set if the following parameters describe a voltage and not a current.</param>
    /// <param name="frequency">Frequency to use.</param>
    /// <param name="range">Range to use - optional followed by scaling.</param>
    /// <param name="percentage">Percentage of range to use - number between 0 and 1.</param>
    /// <param name="detectRange">Set to automatically detect the best range from the reference meter.</param>
    /// <param name="powerFactor">Power factor to use - cosine of the angle between voltage and current.</param>
    Task SetLoadpointAsync(bool isVoltageNotCurrent, string range, double percentage, Frequency frequency, bool detectRange, PowerFactor powerFactor);
}
