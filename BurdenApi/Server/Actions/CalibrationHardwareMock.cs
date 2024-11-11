using BurdenApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Actions;

/// <summary>
/// Simulation helper for the calibration of a burden.
/// </summary>
public class CalibrationHardwareMock : ICalibrationHardware
{
    private const int CorrelationLimit = 127 * 128 + 127;

    private const double FactorLimit = (CorrelationLimit + CorrelationLimit / 10000d) / 123d;

    /// <inheritdoc/>
    public Task<GoalValue> MeasureAsync(Calibration calibration)
    {
        var resCalibration = calibration.Resistive.Coarse * 128d + calibration.Resistive.Fine;
        var indCalibration = calibration.Inductive.Coarse * 128d + calibration.Inductive.Fine;

        var resistence = resCalibration + indCalibration / 1000d;
        var inductive = indCalibration + resCalibration / 1000d;

        var power = resistence / 123d;
        var factor = inductive / 123d;

        return Task.FromResult(new GoalValue(new(power), new(factor / FactorLimit)));
    }

    /// <inheritdoc/>
    public Task SetLoadpointAsync(bool isVoltageNotCurrent, string range, bool detectRange, PowerFactor powerFactor)
    {
        return Task.CompletedTask;
    }
}
