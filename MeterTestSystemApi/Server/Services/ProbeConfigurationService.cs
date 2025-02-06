using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services.Probing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.Models.Logging;
using ZIFApi.Models;

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
                // Load the latest plan.
                var planner = di.GetRequiredService<IConfigurationProbePlan>();
                var ops = await planner.ConfigureProbeAsync();

                logger.LogInformation("processing probing request created at {Created}", ops.Created);

                // Process.
                var result = await planner.FinishProbeAsync(cancel.Token);

                // Nothing more to do.
                if (ops.Request.DryRun) return;

                // Load the current configuration.
                var store = di.GetRequiredService<IMeterTestSystemConfigurationStore>();
                var config = await store.ReadAsync(true);

                // Apply result from probing
                ApplyProbeResult(config, result);

                // Write back.
                await store.WriteAsync(config);
            }
            catch (Exception e)
            {
                logger.LogCritical("unrecoverable error during probing: {Exception}", e.Message);
            }
            finally
            {
                logger.LogInformation("leaving probing mode");

                try
                {
                    // Activate regular operation mode.
                    await di.GetRequiredService<IMeterTestSystemConfigurationStore>().ResetProbingAsync();

                    // Time to restart the server.
                    await di.GetRequiredService<IServerLifetime>().RestartAsync(di.GetRequiredService<IInterfaceLogger>());
                }
                catch (Exception e)
                {
                    logger.LogCritical("failed to finish probing mode properly: {Exception}", e.Message);
                }
            }
        }).Start();
    }

    /// <summary>
    /// Apply the result of a probing to the configuration.
    /// </summary>
    /// <param name="config">Old configuration.</param>
    /// <param name="operation">Proboing result.</param>
    private void ApplyProbeResult(MeterTestSystemConfiguration config, ProbingOperation operation)
    {
        var interfaces = config.Interfaces;
        var request = operation.Request;
        var result = operation.Result;

        // Meter test system.        
        var mts = request.Configuration.FrequencyGenerator ?? request.Configuration.MT768;

        if (mts != null)
        {
            // Reset.
            config.ExternalReferenceMeter = null;
            config.MeterTestSystemType = null;
            config.NoSource = false;

            interfaces.Dosage = null;
            interfaces.MeterTestSystem = null;
            interfaces.ReferenceMeter = null;
            interfaces.SerialPort = null;
            interfaces.Source = null;

            // Check probing.
            if (result.Configuration.FrequencyGenerator != null)
            {
                config.MeterTestSystemType = MeterTestSystemTypes.FG30x;

                interfaces.SerialPort = result.Configuration.FrequencyGenerator.ToLive(SerialProbeProtocols.FG30x);
            }
            else if (result.Configuration.MT768 != null)
            {
                config.MeterTestSystemType = MeterTestSystemTypes.MT786;

                interfaces.SerialPort = result.Configuration.MT768.ToLive(SerialProbeProtocols.FG30x);
            }
        }

        // Error calculator.
        if (request.Configuration.TestPositions.Count > 0)
        {
            // Reset.
            interfaces.ErrorCalculators.Clear();

            // Check probing.            
            var testPositions = result.Configuration.TestPositions;

            for (var pos = 0; pos < testPositions.Count; pos++)
            {
                var testPosition = testPositions[pos];

                if (request.Configuration.TestPositions[pos].EnableMAD)
                {
                    var ip = IPProtocolProvider.GetMadEndpoint((uint)(pos + 1), testPosition.STMServer ?? ServerTypes.STM6000);

                    interfaces.ErrorCalculators.Add(new()
                    {
                        Connection = ErrorCalculatorConnectionTypes.TCP,
                        Endpoint = testPosition.EnableMAD ? $"{ip.Server}:{ip.Port}" : "",
                        Protocol = testPosition.MadProtocol ?? ErrorCalculatorProtocols.MAD_1,
                    });
                }
            }
        }

        // ZIP Socket.
        if (request.Configuration.PM8121ZIF != null)
        {
            // Reset.
            interfaces.ZIFSockets.Clear();

            // Check probing.
            if (result.Configuration.PM8121ZIF != null)
                interfaces.ZIFSockets.Add(new()
                {
                    SerialPort = result.Configuration.PM8121ZIF.ToLive(SerialProbeProtocols.PM8181),
                    Type = SupportedZIFProtocols.PowerMaster8121,
                });
        }

        // Burden.
        if (request.Configuration.ESxB != null)
        {
            // Reset.
            interfaces.Burden.SerialPort = null;
            interfaces.Burden.SimulateHardware = false;

            // Check probing.
            if (result.Configuration.ESxB != null)
                interfaces.Burden = new() { SerialPort = result.Configuration.ESxB.ToLive(SerialProbeProtocols.ESxB) };
        }

        // IP WatchDog.
        if (request.Configuration.EnableIPWatchDog)
        {
            // Reset.
            interfaces.WatchDog.EndPoint = null;

            // Check probing.
            if (result.Configuration.EnableIPWatchDog)
            {
                var ip = IPProtocolProvider.GetIPWatchDogEndpoint();

                interfaces.WatchDog.EndPoint = $"{ip.Server}:{ip.Port}";
            }
        }

        // Barcode reader.
        if (request.Configuration.BarcodeReader != null)
        {
            // Reset.
            interfaces.Barcode = null;

            // Check probing.
            if (result.Configuration.BarcodeReader != null)
            {
                var probe = new HIDProbe { Protocol = HIDProbeProtocols.Barcode, Index = result.Configuration.BarcodeReader.Value };

                interfaces.Barcode = new() { DevicePath = probe.DevicePath };
            }
        }
    }
}