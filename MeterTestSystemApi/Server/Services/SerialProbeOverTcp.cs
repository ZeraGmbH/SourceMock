using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a single serial port probing.
/// </summary>
public class SerialProbeOverTcp : Probe
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    [NotNull, Required]
    public SerialProbeProtocols Protocol { get; set; }

    /// <summary>
    /// Endpoint to use - must include a port.
    /// </summary>
    [NotNull, Required]
    public string Endpoint { get; set; } = null!;

    /// <summary>
    /// Create a description for the probe.
    /// </summary>
    public override string ToString() => $"{Endpoint}: {Protocol}";
}

