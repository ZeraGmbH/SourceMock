using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApi.Models;

/// <summary>
/// Values measured by the burden itself - always
/// single phased.
/// </summary>
public class BurdenValues
{
    /// <summary>
    /// Voltage.
    /// </summary>
    [NotNull, Required]
    public Voltage Voltage { get; set; }

    /// <summary>
    /// Current.
    /// </summary>
    [NotNull, Required]
    public Current Current { get; set; }

    /// <summary>
    /// Angle between current and voltage (in ANSI angular system,
    /// voltage is always 0, angle runs clockwise)
    /// </summary>
    [NotNull, Required]
    public Angle Angle { get; set; }

    /// <summary>
    /// Power factor as cosine of the angle.
    /// </summary>
    [NotNull, Required]
    public PowerFactor PowerFactor { get; set; }

    /// <summary>
    /// Apparent power.
    /// </summary>
    [NotNull, Required]
    public ApparentPower ApparentPower { get; set; }
}