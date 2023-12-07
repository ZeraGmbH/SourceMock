using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Model;

/// <summary>
/// 
/// </summary>
[Serializable]
public class MeterTestSystemFirmwareVersion
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [NotNull]
    public string ModelName { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [NotNull]
    public string Version { get; set; } = null!;
}
