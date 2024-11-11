using BurdenApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Actions;

/// <summary>
/// Implementation of a calibration environment.
/// </summary>
public class CalibrationHardware : ICalibrationHardware
{
    /// <inheritdoc/>
    public Task<GoalValue> MeasureAsync(Calibration calibration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetLoadpointAsync(bool isVoltageNotCurrent, string range, bool detectRange, PowerFactor powerFactor)
    {
        throw new NotImplementedException();
    }
}