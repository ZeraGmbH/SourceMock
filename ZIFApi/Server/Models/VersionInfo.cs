using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ZIFApi.Models;

/// <summary>
/// 
/// </summary>
public class ZIFVersionInfo
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public SupportedZIFProtocols Protocol { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public int Major { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public int Minor { get; set; }
}