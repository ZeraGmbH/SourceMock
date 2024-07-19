using MeterTestSystemApi.Models.ConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Service to scan the system for meter test system components.
/// </summary>
/// <remarks>This is NOT a real implemntation esp. concerning synchronisation. This is another story!</remarks>
public class ProbeConfigurationService : IProbeConfigurationService
{
    private class Current
    {
        /// <summary>
        /// Current executing probe task.
        /// </summary>
        private TaskCompletionSource _probing = new();

        /// <summary>
        /// Current cancel token.
        /// </summary>
        private CancellationTokenSource _cancel = new();

        /// <summary>
        /// Send a cancel to the probing task.
        /// </summary>
        public Task Cancel() => _cancel.CancelAsync();

        /// <summary>
        /// Report the probing task.
        /// </summary>
        public Task Task => _probing.Task;
    }

    /// <summary>
    /// Synchronize access to probe operation.
    /// </summary>
    private readonly object _sync = new();

    /// <inheritdoc/>
    public bool IsActive => _active?.Task.IsCompleted == false;

    /// <summary>
    /// Result of the last probing.
    /// </summary>
    private volatile ProbeConfigurationResult? _lastResult;

    /// <inheritdoc/>
    public ProbeConfigurationResult? Result => _lastResult;

    /// <summary>
    /// Current executing probe task.
    /// </summary>
    private Current? _active;

    /// <inheritdoc/>
    public async Task Abort()
    {
        /* Request the current probing task. */
        Current? active;

        lock (_sync) active = _active;

        /* See if there is some active task. */
        if (active == null) throw new InvalidOperationException("no active probing");

        /* Send a cancel request. */
        await active.Cancel();

        /* Wait for the task to cancel. */
        await active.Task;
    }

    /// <inheritdoc/>
    public Task StartProbe(ProbeConfigurationRequest request, bool dryRun, IServiceProvider services)
    {
        lock (_sync)
        {
            /* There must be no active probing. */
            if (_active != null) throw new InvalidOperationException("probing already active");

            /* Create the plan. */
            var plan = services.GetRequiredService<IConfigurationProbePlan>();

            /* Fill the probes. */
            plan.ConfigureProbe(request).Wait();

            /* Copy plan steps to result. */
            var result = plan.FinishProbe().Result;

            /* Only dry run allowed. */
            if (dryRun)
            {
                /* Provide as last result. */
                _lastResult = result;

                /* Finish. */
                return Task.CompletedTask;
            }

            /* Create the new probing task. */
            _active = new();
        }


        throw new NotSupportedException("for now only dry run possible");
    }
}