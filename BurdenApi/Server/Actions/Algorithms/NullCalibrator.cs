using BurdenApi.Models;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// Calibration algorithm just measuring.
/// </summary>
public class NullCalibrator : ICalibrationAlgorithm
{
    /// <inheritdoc/>
    public Calibration CreateInitialCalibration(Calibration calibration) => calibration;

    /// <inheritdoc/>
    public Task<CalibrationStep?> IterateAsync(ICalibrationContext context) => Task.FromResult<CalibrationStep?>(null);

    /// <inheritdoc/>
    public bool ContinueAfterCycleDetection() => false;
}