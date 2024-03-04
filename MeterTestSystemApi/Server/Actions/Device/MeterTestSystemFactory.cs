using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implement a meter test system factory.
/// </summary>
/// <param name="services">Dependency injection to use.</param>
public class MeterTestSystemFactory(IServiceProvider services) : IMeterTestSystemFactory
{
    private readonly object _sync = new();

    private IMeterTestSystem? _meterTestSystem;

    /// <inheritdoc/>
    public IMeterTestSystem MeterTestSystem
    {
        get
        {
            /* Wait until instance has been created. */
            while (_meterTestSystem == null)
                lock (_sync)
                    Monitor.Wait(_sync);

            return _meterTestSystem;
        }
    }

    /// <inheritdoc/>
    public void Inititalize(MeterTestSystemConfiguration configuration)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_meterTestSystem != null) throw new InvalidOperationException("Meter test system already initialized");

            /* Create it depending on the configuration. */
            IMeterTestSystem meterTestSystem;

            switch (configuration.MeterTestSystemType)
            {
                case MeterTestSystemTypes.MT786:
                    meterTestSystem = services.GetRequiredService<SerialPortMTMeterTestSystem>();
                    break;
                case MeterTestSystemTypes.FG30x:
                    meterTestSystem = services.GetRequiredService<SerialPortFGMeterTestSystem>();
                    break;
                case MeterTestSystemTypes.Mock:
                    meterTestSystem = services.GetRequiredService<MeterTestSystemMock>();
                    break;
                default:
                    meterTestSystem = services.GetRequiredService<FallbackMeteringSystem>();
                    break;
            }

            /* Use the new instance. */
            _meterTestSystem = meterTestSystem;

            /* Signal availability of meter test system. */
            Monitor.PulseAll(_sync);
        }
    }
}
