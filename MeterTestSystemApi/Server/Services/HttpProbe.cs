using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a HTTP endpoint probing.
/// </summary>
public class HttpProbe : Probe
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    [NotNull, Required]
    public IPProbeProtocols Protocol { get; set; }

    /// <summary>
    /// IP endpoint to use.
    /// </summary>
    [NotNull, Required]
    public string EndPoint { get; set; } = null!;

    /// <summary>
    /// Create a description for the probe.
    /// </summary>
    public override string ToString() => $"{EndPoint}[{Protocol}]";
}

