using System.Net;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a single IP probing.
/// </summary>
internal class IPProbe : Probe
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    public required IPProbeProtocols Protocol { get; set; }

    /// <summary>
    /// IP endpoint to use.
    /// </summary>
    public required IPEndPoint EndPoint { get; set; }

    /// <summary>
    /// Create a description for the probe.
    /// </summary>
    public override string ToString() => $"{EndPoint}[{Protocol}]";
}

