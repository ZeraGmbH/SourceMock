using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Helper class for calculating currents
/// </summary>
public static class CurrentCalculation
{
    /// <summary>
    /// returns the total power of given ac phase
    /// </summary>
    /// <param name="phase"></param>
    /// <returns></returns>
    public static ActivePower CalculateAcPower<TVoltage, TCurrent>(AbstractLoadpointPhase<TVoltage, TCurrent> phase)
        where TVoltage : ElectricalQuantity<Voltage>, new()
        where TCurrent : ElectricalQuantity<Current>, new()
    {
        var acVoltage = phase.Voltage.AcComponent;
        var acCurrent = phase.Current.AcComponent;

        if (acCurrent != null && acVoltage != null)
            return (acVoltage.Rms * acCurrent.Rms).GetActivePower(acVoltage.Angle - acCurrent.Angle);

        return ActivePower.Zero;
    }

    /// <summary>
    /// returns the total power of given dc phase
    /// </summary>
    /// <param name="phase"></param>
    /// <returns></returns>
    public static ActivePower CalculateDcPower<TVoltage, TCurrent>(AbstractLoadpointPhase<TVoltage, TCurrent> phase)
        where TVoltage : ElectricalQuantity<Voltage>, new()
        where TCurrent : ElectricalQuantity<Current>, new()
    {
        var dcVoltage = phase.Voltage.DcComponent;
        var dcCurrent = phase.Current.DcComponent;

        if (dcCurrent != null && dcVoltage != null)
            return (dcVoltage.Value * dcCurrent.Value).GetActivePower(Angle.Zero);

        return ActivePower.Zero;
    }
}