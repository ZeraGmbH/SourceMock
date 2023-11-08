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
    /// Remaining count for the current measurment.
    /// </summary>
    public long Remaining { get; set; }

    /// <summary>
    /// Count provided so far in the current measurement.
    /// </summary>
    public long Progress { get; set; }
}
