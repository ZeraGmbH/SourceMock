using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// 
/// </summary>
public class SerialPortReply
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public List<string> Matches { get; set; } = [];
}
