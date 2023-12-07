using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RefMeterApi.Models;
using SourceApi.Model;

namespace MeterTestSystemApi.Models;

/// <summary>
/// 
/// </summary>
public class AmplifiersAndReferenceMeters
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public VoltageAmplifiers VoltageAmplifier { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public VoltageAuxiliaries VoltageAuxiliary { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public CurrentAmplifiers CurrentAmplifier { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public CurrentAuxiliaries CurrentAuxiliary { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public ReferenceMeters ReferenceMeter { get; set; }
}
