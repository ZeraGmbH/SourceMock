using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using RefMeterApi.Models;
using SourceApi.Model;

namespace FrequencyGeneratorApi.Models;

/// <summary>
/// 
/// </summary>
public class FrequencyGeneratorCapabilities
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public List<VoltageAmplifiers> SupportedVoltageAmplifiers { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public List<CurrentAmplifiers> SupportedCurrentAmplifiers { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public List<ReferenceMeters> SupportedReferenceMeters { get; set; } = new();
}
