namespace MeterTestSystemApi.Services;

/// <summary>
/// Interface implemented by an algorithm which can probe something.
/// </summary>
public interface IProbeExecutor
{
}

/// <summary>
/// Algorithm to probe through specific types of hardware connection.
/// </summary>
/// <typeparam name="T">Type of the configuration.</typeparam>
public interface IProbeExecutor<T> : IProbeExecutor where T : Probe
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Probe configuration.</param>
    /// <returns>Error message or empty.</returns>
    Task<string> ExecuteAsync(T probe);
}

/// <summary>
/// Helper class to simplify using generic algoritms.
/// </summary>
public static class IProbeExecutorExtensions
{
    /// <summary>
    /// Execute an algorithm.
    /// </summary>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="probe">Probing configuration.</param>
    /// <typeparam name="T">Type of the probing configuration.</typeparam>
    /// <returns>Empty or error code.</returns>
    private static Task<string> ExecuteTypedAsync<T>(this IProbeExecutor algorithm, T probe) where T : Probe
        => ((IProbeExecutor<T>)algorithm).ExecuteAsync((T)probe);

    /// <summary>
    /// Execute an algorith.
    /// </summary>
    /// <param name="algorithm">Some algorithm.</param>
    /// <param name="probe">Probe configuration.</param>
    /// <returns>Empty or error code.</returns>
    public static Task<string> ExecuteAsync(this IProbeExecutor algorithm, Probe probe)
        => (Task<string>)algorithm.GetType().GetMethod(nameof(IProbeExecutor<Probe>.ExecuteAsync))!.Invoke(algorithm, [probe])!;
}