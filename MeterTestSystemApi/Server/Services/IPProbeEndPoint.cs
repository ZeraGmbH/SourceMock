using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Services;

/// <summary>
/// 
/// </summary>
public class IPProbeEndPoint
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string Server { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public ushort Port { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"{Server}:{Port}";
}

