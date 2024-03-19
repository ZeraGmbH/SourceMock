using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedLibrary.Models;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implement a meter test system factory.
/// </summary>
/// <param name="services">Dependency injection to use.</param>
/// <param name="factory">Factory to create error calculators.</param>
/// <param name="logger">Logging helper.</param>
public class MeterTestSystemFactory(IServiceProvider services, IErrorCalculatorFactory factory, ILogger<MeterTestSystemFactory> logger) : IMeterTestSystemFactory, IDisposable
{
    private readonly object _sync = new();

    private bool _initialized = false;

    private IMeterTestSystem? _meterTestSystem;

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
            _initialized = true;
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
                        {
                            var meterTestSystem = services.GetRequiredService<SerialPortFGMeterTestSystem>();

                            _meterTestSystem = meterTestSystem;

                            meterTestSystem.ConfigureErrorCalculators(configuration.Interfaces.ErrorCalculators, factory).Wait();

                            if (configuration.AmplifiersAndReferenceMeter != null)
                                try
                                {
                                    /* Do all configurations. */
                                    _meterTestSystem
                                        .SetAmplifiersAndReferenceMeter(configuration.AmplifiersAndReferenceMeter)
                                        .Wait();
                                }
                                catch (Exception e)
                                {
                                    /* Just report - let meter test system run. */
                                    logger.LogError("Unable to restore amplifiers: {Exception}", e.Message);
                                }

                            break;
                        }
                    case MeterTestSystemTypes.REST:
                        {
                            var meterTestSystem = services.GetRequiredService<RestMeterTestSystem>();

                            meterTestSystem.Configure(configuration.Interfaces, services);

                            _meterTestSystem = meterTestSystem;
                            break;
                        }
                    case MeterTestSystemTypes.MT786:
                        _meterTestSystem = services.GetRequiredService<SerialPortMTMeterTestSystem>();
                        break;
                    case MeterTestSystemTypes.ACMock:
                        _meterTestSystem = services.GetRequiredService<MeterTestSystemAcMock>();
                        break;
                    case MeterTestSystemTypes.DCMock:
                        _meterTestSystem = services.GetRequiredService<MeterTestSystemDcMock>();
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
}
