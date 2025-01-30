using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="endPoint"></param>
    public static implicit operator IPProbeEndPoint(IPEndPoint endPoint) => new() { Server = endPoint.Address.ToString(), Port = (ushort)endPoint.Port };
}

