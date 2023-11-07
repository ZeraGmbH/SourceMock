namespace RefMeterApi.Models;

/// <summary>
/// Report the progress of the latests dosage measurement.
/// </summary>
public class DosageProgress
{
    /// <summary>
    /// Remaining energy for the current measurment.
    /// </summary>
    public double Remaining { get; set; }

    /// <summary>
    /// Energy provided so far in the current measurement.
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// Total energy for the current measurement.
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// Unit to use for all energy values.
    /// </summary>
    public EnergyUnits Unit { get; set; }
}
