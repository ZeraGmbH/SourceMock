using SharedLibrary.DomainSpecific;
using SourceApi.Model;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Helper class for calculating currents
/// </summary>
public static class CurrentCalculation
{
    /// <summary>
    /// returns the total power of given ac phase
    /// </summary>
    /// <param name="totalPower"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    public static ActivePower CalculateAcPower(ActivePower totalPower, TargetLoadpointPhase phase)
    {
        var acVoltage = phase.Voltage.AcComponent;
        var acCurrent = phase.Current.AcComponent;
        if (acCurrent != null && acVoltage != null)
            totalPower += (acVoltage.Rms * acCurrent.Rms).GetActivePower(acVoltage.Angle - acCurrent.Angle);
        return totalPower;
    }

    /// <summary>
    /// returns the total power of given dc phase
    /// </summary>
    /// <param name="totalPower"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    public static ActivePower CalculateDcPower(ActivePower totalPower, TargetLoadpointPhase phase)
    {
        var dcVoltage = phase.Voltage.DcComponent;
        var dcCurrent = phase.Current.DcComponent;
        if (dcCurrent != null && dcVoltage != null)
        {
            var apparentPower = dcVoltage.Value * dcCurrent.Value;
            totalPower = apparentPower.GetActivePower(new Angle());
        }
        return totalPower;
    }
}