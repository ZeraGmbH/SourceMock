namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Information of a probing.
/// </summary>
public class ProbeInfo
{
    /// <summary>
    /// Set when the probe succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// In case of any error the error message else just some optional information.
    /// </summary>
    public string? Message { get; set; }
}
