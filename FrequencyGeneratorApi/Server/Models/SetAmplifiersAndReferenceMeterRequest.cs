using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RefMeterApi.Models;
using SourceApi.Model;

namespace FrequencyGeneratorApi.Models;

/// <summary>
/// 
/// </summary>
public class SetAmplifiersAndReferenceMeterRequest
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
    public CurrentAmplifiers CurrentAmplifier { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public ReferenceMeters ReferenceMeter { get; set; }
}
