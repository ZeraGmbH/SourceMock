using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebSamDeviceApis.Model;

/// <summary>
/// Report the progress of the latest dosage measurement.
/// </summary>
public class DosageProgress
{
    /// <summary>
    /// Set if a dosage measurement is currently active.
    /// </summary>
    [Required, NotNull]
    public bool Active { get; set; }

    /// <summary>
    /// Remaining energy for the current measurment in Wh.
    /// </summary>
    [Required, NotNull]
    public double Remaining { get; set; }

    /// <summary>
    /// Energy provided so far in the current measurement in Wh.
    /// </summary>
    [Required, NotNull]
    public double Progress { get; set; }

    /// <summary>
    /// Total number of requested energy in Wh.
    /// </summary>
    [Required, NotNull]
    public double Total { get; set; }
}
