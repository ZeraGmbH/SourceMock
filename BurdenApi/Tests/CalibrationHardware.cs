using BurdenApi.Models;

namespace BurdenApiTests;

public class CalibrationHardware : ICalibrationHardware
{
    private const int CorrelationLimit = 127 * 128 + 127;

    private const double FactorLimit = CorrelationLimit / 123d;

    public Task<GoalValue> MeasureAsync(Calibration calibration)
    {
        var resistance = calibration.Resistive.Coarse * 128 + calibration.Resistive.Fine;
        var inductive = calibration.Inductive.Coarse * 128 + calibration.Inductive.Fine;

        var power = resistance / 123d;
        var factor = inductive / 123d;

        return Task.FromResult(new GoalValue(new(power), new(factor / FactorLimit)));
    }
}
