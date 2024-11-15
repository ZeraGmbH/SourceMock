using BurdenApi.Actions.Algorithms;

namespace BurdenApi.Models;

/// <summary>
/// Describes a calibrator algorithm.
/// </summary>
public interface ICalibrator
{
    /// <summary>
    /// Initial calibration used to calibrate.
    /// </summary>
    Calibration InitialCalibration { get; }

    /// <summary>
    /// Target of the calibration.
    /// </summary>
    GoalValue Goal { get; }

    /// <summary>
    /// Report all steps needed to reach the goal.
    /// </summary>
    CalibrationStep[] Steps { get; }

    /// <summary>
    /// Report the final step of the calibration holding the result as well.
    /// </summary>
    CalibrationStep? LastStep { get; }

    /// <summary>
    /// Process a calibration.
    /// </summary>
    /// <param name="request">Configuration of the calibration algorithm.</param>
    /// <param name="cancel">Allows to cancel the algorithm as soon as possible.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    Task RunAsync(CalibrationRequest request, CancellationToken cancel, CalibrationAlgorithms algorithm = CalibrationAlgorithms.Default);

    /// <summary>
    /// Process a calibration, write the result back to the burden and
    /// validate at 80% and 120%.
    /// </summary>
    /// <param name="request">Configuration of the calibration algorithm.</param>
    /// <param name="cancel">Allows to cancel the algorithm as soon as possible.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <returns>Details on the calibration.</returns>
    Task<CalibrationStep[]> CalibrateStepAsync(CalibrationRequest request, CancellationToken cancel, CalibrationAlgorithms algorithm = CalibrationAlgorithms.Default);
}
