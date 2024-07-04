using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

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
    public ActiveEnergy Remaining { get; set; }

    /// <summary>
    /// Energy provided so far in the current measurement in Wh.
    /// </summary>
    [Required, NotNull]
    public ActiveEnergy Progress { get; set; }

    /// <summary>
    /// Total number of requested energy in Wh.
    /// </summary>
    [Required, NotNull]
    public ActiveEnergy Total { get; set; }
}
