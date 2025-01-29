using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services.Probing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Service to scan the system for meter test system components.
/// </summary>
public class ProbeConfigurationService(IServerLifetime lifetime, IActiveOperations activities, IServiceProvider services, ILogger<ProbeConfigurationService> logger) : IProbeConfigurationService
{
    /// <summary>
    /// Synchronize access to probe operation.
    /// </summary>
    private readonly object _sync = new();

    /// <inheritdoc/>
    public bool IsActive => _active != null;

    /// <summary>
    /// Test only.
    /// </summary>
    public void ResetActive() => _active = null;

    /// <summary>
    /// Current executing probe task.
    /// </summary>
    private CancellationTokenSource? _active;

    /// <inheritdoc/>
    public void Abort()
    {
        lock (_sync)
            if (_active == null)
                throw new InvalidOperationException("no active probing");
            else
                _active.Cancel();
    }

    /// <inheritdoc/>
    public async Task ConfigureProbingAsync(ProbeConfigurationRequest request, IServiceProvider services)
    {
        lock (_sync)
        {
            /* There must be no active probing. */
            if (_active != null) throw new InvalidOperationException("probing already active");

            /* Create the plan. */
            var store = services.GetRequiredService<IProbingOperationStore>();

            /* Write plan to database. */
#pragma warning disable VSTHRD103 // Call async methods when in an async method
            store.AddAsync(new()
            {
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Request = request,
                Result = new(),
            }).Wait();
#pragma warning restore VSTHRD103 // Call async methods when in an async method

            /* Lock out. */
            _active = new();
        }

        /* Mark probing as active - actually not of much value sind since server will be restarted soon. */
        activities.SetOperation(ActiveOperationTypes.Probing, true);

        /* Activate probing. */
        await services.GetRequiredService<IMeterTestSystemConfigurationStore>().StartProbingAsync();

        /* Restart server. */
        await lifetime.RestartAsync(services.GetRequiredService<IInterfaceLogger>());
    }

    /// <inheritdoc/>
    public async Task<ProbeInfo[]> ProbeManualAsync(IEnumerable<Probe> probes, IServiceProvider services)
    {
        var errors = new List<ProbeInfo>();

        foreach (var probe in probes)
            try
            {
                if (probe.Result == ProbeResult.Skipped)
                    errors.Add(new() { Succeeded = true, Message = "Excluded from probing" });
                else
                    errors.Add(await
                        services
                            .GetRequiredKeyedService<IProbeExecutor>(probe.GetType())
                            .ExecuteAsync(probe));
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException dyn) e = dyn.InnerException ?? e;

                errors.Add(new() { Message = $"{probe}: {e.Message}", Succeeded = false });
            }

        return [.. errors];
    }

    /// <summary>
    /// Start probing on a separate thread.
    /// </summary>
    public void StartProbing()
    {
        new Thread(async () =>
        {
            // Mark as probing.
            var cancel = new CancellationTokenSource();

            _active = cancel;

            // Lockout client since we are now probing.
            activities.SetOperation(ActiveOperationTypes.Probing, true);

            // Report.
            logger.LogInformation("entering probing mode");

            // Create probe esp. to access interface logging and databases.
            using var scope = services.CreateScope();

            var di = scope.ServiceProvider;

            try
            {
                // Interface logger used for all hardware access.
                var interfaceLogger = di.GetRequiredService<IInterfaceLogger>();
                var store = di.GetRequiredService<IConfigurationProbePlan>();

                try
                {
                    // Load the latest plan.
                    var ops = await store.ConfigureProbeAsync();

                    logger.LogInformation("processing probing request created at {Created}", ops.Created);

                    // Process.
                    await store.FinishProbeAsync(cancel.Token);
                }
                finally
                {
                    logger.LogInformation("leaving probing mode");

                    try
                    {
                        // Activate regular operation mode.
                        await di.GetRequiredService<IMeterTestSystemConfigurationStore>().ResetProbingAsync();

                        // Time to restart the server.
                        await di.GetRequiredService<IServerLifetime>().RestartAsync(interfaceLogger);
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical("failed to finish probing mode properly: {Exception}", e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogCritical("unrecoverable error during probing: {Exception}", e.Message);
            }
        }).Start();
    }
}