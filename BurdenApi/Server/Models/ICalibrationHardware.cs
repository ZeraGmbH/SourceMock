using RefMeterApi.Actions.Device;
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
    /// Prepare the measurement.
    /// </summary>
    /// <param name="frequency">Frequency to use.</param>
    /// <param name="range">Range to use - optional followed by scaling.</param>
    /// <param name="percentage">Percentage of range to use - 1 means take range as is in the loadpoint.</param>
    /// <param name="detectRange">Set to automatically detect the best range from the reference meter.</param>
    /// <param name="goal">Current step of the calibration, includes apparent power and power factor.</param>
    Task PrepareAsync(string range, double percentage, Frequency frequency, bool detectRange, GoalValue goal);

    /// <summary>
    /// Report the burden associated with this hardware.
    /// </summary>
    IBurden Burden { get; }
}
