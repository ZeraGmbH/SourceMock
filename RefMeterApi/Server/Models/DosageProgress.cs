namespace RefMeterApi.Models;

/// <summary>
/// Report the progress of the latest dosage measurement.
/// </summary>
public class DosageProgress
{
    /// <summary>
    /// Set if a dosage measurement is currently active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Remaining energy for the current measurment in Wh.
    /// </summary>
    public double Remaining { get; set; }

    /// <summary>
    /// Energiy provided so far in the current measurement in Wh.
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// Total number of requested energy in Wh.
    /// </summary>
    public double Total { get; set; }
}
