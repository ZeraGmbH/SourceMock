using RefMeterApi.Models;
using SourceApi.Model;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// Physical configuration of a meter test system - not
/// all types of tests systems allow configuraton.
/// </summary>
public class AmplifiersAndReferenceMeters
{
    /// <summary>
    /// The voltage amplifier connected to the meter test system.
    /// </summary>
    [NotNull, Required]
    public VoltageAmplifiers VoltageAmplifier { get; set; }

    /// <summary>
    /// The auxiliary voltage amplifier connected to the meter test system.
    /// </summary>
    [NotNull, Required]
    public VoltageAuxiliaries VoltageAuxiliary { get; set; }

    /// <summary>
    /// The current amplifier connected to the meter test system.
    /// </summary>
    [NotNull, Required]
    public CurrentAmplifiers CurrentAmplifier { get; set; }

    /// <summary>
    /// The auxiliary current amplifier connected to the meter test system.
    /// </summary>
    [NotNull, Required]
    public CurrentAuxiliaries CurrentAuxiliary { get; set; }

    /// <summary>
    /// The reference meter connected to the meter test system.
    /// </summary>
    [NotNull, Required]
    public ReferenceMeters ReferenceMeter { get; set; }
}
