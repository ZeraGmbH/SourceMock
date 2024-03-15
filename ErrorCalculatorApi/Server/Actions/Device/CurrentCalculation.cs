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
    public static double CalculateAcPower(double totalPower, TargetLoadpointPhase phase)
    {
        var acVoltage = phase.Voltage.AcComponent;
        var acCurrent = phase.Current.AcComponent;
        if (acCurrent != null && acVoltage != null)
            totalPower += acVoltage.Rms * acCurrent.Rms *
            Math.Cos((acVoltage.Angle - acCurrent.Angle) * Math.PI / 180d);
        return totalPower;
    }

    /// <summary>
    /// returns the total power of given dc phase
    /// </summary>
    /// <param name="totalPower"></param>
    /// <param name="phase"></param>
    /// <returns></returns>
    public static double CalculateDcPower(double totalPower, TargetLoadpointPhase phase)
    {
        var dcVoltage = phase.Voltage.DcComponent;
        var dcCurrent = phase.Current.DcComponent;
        if (dcCurrent != null && dcVoltage != null)
            totalPower += (double)dcVoltage * (double)dcCurrent;
        return totalPower;
    }
}