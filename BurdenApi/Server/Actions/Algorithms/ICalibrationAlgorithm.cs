using BurdenApi.Models;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// Algorithm to do a 
/// </summary>
public interface ICalibrationAlgorithm
{
    /// <summary>
    /// Create the initial calibration.
    /// </summary>
    /// <param name="calibration">Calibration as reported by the burden.</param>
    /// <returns>Calibration to use as startup.</returns>
    Calibration CreateInitialCalibration(Calibration calibration);

    /// <summary>
    /// Execute one calibration iteration step.
    /// </summary>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>Successfull calibration step or null.</returns>
    Task<CalibrationStep?> IterateAsync(ICalibrationContext context);

    /// <summary>
    /// A cycle has been detected.
    /// </summary>
    /// <returns>Set if the calibration can be continued with the best fit so far.</returns>
    bool ContinueAfterCycleDetection();
}
