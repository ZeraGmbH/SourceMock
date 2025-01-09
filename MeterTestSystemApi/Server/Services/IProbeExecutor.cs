using MeterTestSystemApi.Actions.Probing;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Interface implemented by an algorithm which can probe something.
/// </summary>
public interface IProbeExecutor
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Probe configuration.</param>
    /// <returns>Error message or empty.</returns>
    Task<ProbeInfo> ExecuteAsync(Probe probe);
}

/// <summary>
/// Algorithm to probe through specific types of hardware connection.
/// </summary>
/// <typeparam name="T">Type of the configuration.</typeparam>
public abstract class ProbeExecutor<T> : IProbeExecutor where T : Probe
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Probe configuration.</param>
    /// <returns>Error message or empty.</returns>
    protected abstract Task<ProbeInfo> OnExecuteAsync(T probe);

    /// <inheritdoc/>
    public Task<ProbeInfo> ExecuteAsync(Probe probe) => OnExecuteAsync((T)probe);
}
