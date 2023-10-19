using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebSamDeviceApis.Model;

/// <summary>
/// 
/// </summary>
[Serializable]
public class DeviceFirmwareVersion
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
