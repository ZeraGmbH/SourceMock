namespace MeterTestSystemApi.Services;

internal abstract class Probe
{
    /// <summary>
    /// Set the result of the probing.
    /// </summary>
    public ProbeResult Result { get; set; }
}