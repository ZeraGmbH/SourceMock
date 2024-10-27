using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// 
/// </summary>
public class SerialPortCommand
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string Command { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string Reply { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public bool UseRegularExpression { get; set; }
}
