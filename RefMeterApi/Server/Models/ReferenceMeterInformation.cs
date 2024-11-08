using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.DomainSpecific;

namespace RefMeterApi.Models;

/// <summary>
/// Firmware information on reference meter.
/// </summary>
[Serializable]
public class ReferenceMeterInformation
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string Model { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string SoftwareVersion { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Voltage? MaximumDCVoltage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Voltage? MaximumACVoltage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Current? MaximumDCCurrent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Current? MaximumACCurrent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public uint NumberOfPhases { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<ReferenceMeterEMOBSessions>? SupportedEMOBSessions { get; set; }

    /// <summary>
    /// Set if the reference meter allows to choose ranges.
    /// </summary>
    public bool SupportsManualRanges { get; set; }

    /// <summary>
    /// Set if the reference meter allows to select a PLL channel.
    /// </summary>
    public bool SupportsPllChannelSelection { get; set; }
}



