using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implement a meter test system factory.
/// </summary>
/// <param name="services">Dependency injection to use.</param>
/// <param name="factory">Factory to create error calculators.</param>
/// <param name="logger">Logging helper.</param>
/// <param name="lifetime">Lifetime control.</param>
public class MeterTestSystemFactory(IServiceProvider services, IErrorCalculatorFactory factory, ILogger<MeterTestSystemFactory> logger, IServerLifetime lifetime) : IMeterTestSystemFactory, IDisposable
{
    private readonly object _sync = new();

    private bool _initialized = false;

    private IMeterTestSystem? _meterTestSystem;

    private readonly List<IDisposable?> _Disposables = [];

    /// <inheritdoc/>
    public IMeterTestSystem MeterTestSystem
    {
        get
        {
            /* Wait until instance has been created. */
            lock (_sync)
                while (!_initialized)
                    Monitor.Wait(_sync);

            return _meterTestSystem!;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_sync)
        {
            _Disposables.ForEach(d => d?.Dispose());

            _initialized = true;
        }
    }

    /// <inheritdoc/>
    public void Initialize(MeterTestSystemConfiguration configuration)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_initialized) throw new InvalidOperationException("Meter test system already initialized");

            try
            {
                /* Create it depending on the configuration. */
                switch (configuration.MeterTestSystemType)
                {
                    case MeterTestSystemTypes.FG30x:
                        ConfigureFG30x(configuration).Wait();
                        break;
                    case MeterTestSystemTypes.REST:
                        ConfigureREST(configuration).Wait();
                        break;
                    case MeterTestSystemTypes.MT786:
                        ConfigureMT786(configuration);
                        break;
                    case MeterTestSystemTypes.ACMock:
                    case MeterTestSystemTypes.ACMockNoSource:
                        ConfigureACMock(configuration);
                        break;
                    case MeterTestSystemTypes.DCMockNoSource:
                    case MeterTestSystemTypes.DCMock:
                        ConfigureDCMock(configuration);
                        break;
                    default:
                        _meterTestSystem = services.GetRequiredService<FallbackMeteringSystem>();
                        break;
                }
            }
            catch (Exception e)
            {
                logger.LogCritical("Unable to create meter test system: {Exception}", e.Message);
            }
            finally
            {
                /* Use fallback so that there is ALWAYS a meter test system. */
                _meterTestSystem ??= services.GetRequiredService<FallbackMeteringSystem>();

                /* Use the new instance. */
                _initialized = true;

                /* Signal availability of meter test system. */
                Monitor.PulseAll(_sync);
            }
        }
    }

    private void ConfigureDCMock(MeterTestSystemConfiguration configuration)
    {
        var meterTestSystem = services.GetRequiredService<MeterTestSystemDcMock>();

        if (configuration.MeterTestSystemType == MeterTestSystemTypes.DCMockNoSource || configuration.NoSource == true)
            meterTestSystem.NoSource();

        _meterTestSystem = meterTestSystem;
    }

    private void ConfigureACMock(MeterTestSystemConfiguration configuration)
    {
        var meterTestSystem = services.GetRequiredService<MeterTestSystemAcMock>();

        if (configuration.MeterTestSystemType == MeterTestSystemTypes.ACMockNoSource || configuration.NoSource == true)
            meterTestSystem.NoSource();

        _meterTestSystem = meterTestSystem;
    }

    private void ConfigureMT786(MeterTestSystemConfiguration configuration)
    {
        var meterTestSystem = services.GetRequiredService<SerialPortMTMeterTestSystem>();

        if (configuration.NoSource == true)
            meterTestSystem.NoSource();

        _meterTestSystem = meterTestSystem;
    }

    private async Task ConfigureREST(MeterTestSystemConfiguration configuration)
    {
        var meterTestSystem = services.GetRequiredService<RestMeterTestSystem>();

        _meterTestSystem = meterTestSystem;

        await meterTestSystem.ConfigureAsync(configuration.Interfaces, services, new NoopInterfaceLogger());
    }

    private async Task ConfigureFG30x(MeterTestSystemConfiguration configuration)
    {
        var meterTestSystem = services.GetRequiredService<SerialPortFGMeterTestSystem>();

        if (configuration.NoSource == true)
            meterTestSystem.NoSource();

        _meterTestSystem = meterTestSystem;

        await meterTestSystem.ActivateErrorConditionsAsync(new NoopInterfaceLogger());
        await meterTestSystem.ConfigureErrorCalculatorsAsync(configuration.Interfaces.ErrorCalculators, factory);

        // May need to preset amplifiers.
        if (configuration.AmplifiersAndReferenceMeter != null)
            try
            {
                /* Do all configurations. */
                await _meterTestSystem.SetAmplifiersAndReferenceMeterAsync(new NoopInterfaceLogger(), configuration.AmplifiersAndReferenceMeter);
            }
            catch (Exception e)
            {
                /* Just report - let meter test system run. */
                logger.LogError("Unable to restore amplifiers: {Exception}", e.Message);
            }

        await meterTestSystem.InitializeFG(new NoopInterfaceLogger());

        // Requires cleanup.
        lifetime.AddCleanup(meterTestSystem.DeinitializeFG);

        // May want to create an external reference meter.
        _Disposables.Add(meterTestSystem.ConfigureExternalReferenceMeterAsync(configuration.ExternalReferenceMeter));
    }
}
