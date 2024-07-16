namespace MeterTestSystemApi.Services;

internal enum ProbeResult
{
    /// <summary>
    /// Planned to probe.
    /// </summary>
    Planned,

    /// <summary>
    /// Probing successful.
    /// </summary>
    Succeeded,

    /// <summary>
    /// Probing failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Probing skipped - no result.
    /// </summary>
    Skipped
}

