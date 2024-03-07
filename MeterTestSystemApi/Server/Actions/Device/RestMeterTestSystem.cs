using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Actions.Device;
using SourceApi.Actions.RestSource;
using SourceApi.Actions.Source;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Meter test system based on REST connection to source and reference meter.
/// </summary>
public class RestMeterTestSystem : IMeterTestSystem
{
    /// <inheritdoc/>
    public AmplifiersAndReferenceMeter AmplifiersAndReferenceMeter => throw new NotImplementedException();

    /// <inheritdoc/>
    public ISource Source { get; private set; } = new UnavailableSource();

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; } = new UnavailableReferenceMeter();

    private readonly List<IErrorCalculator> _errorCalculators = [new UnavailableErrorCalculator()];

    /// <inheritdoc/>
    public IErrorCalculator[] ErrorCalculators => _errorCalculators.ToArray();

    /// <inheritdoc/>
#pragma warning disable CS0414
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilities() =>
        Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditions() =>
        Task.FromResult(new ErrorConditions());

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion() =>
        Task.FromResult(new MeterTestSystemFirmwareVersion
        {
            ModelName = "REST",
            Version = "0.1"
        });

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeter settings)
    {
        /* The fallback do not support amplifier configurations. */
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Configure all sub components.
    /// </summary>
    /// <param name="config">Configuration to use.</param>
    /// <param name="di">Active dependency injection runtime to create helper.</param>
    public void Configure(InterfaceConfiguration config, IServiceProvider di)
    {
        /* Validate. */
        if (config.Source == null) throw new InvalidOperationException("no source connection configured");

        /* Create. */
        var source = di.GetRequiredService<IRestSource>();

        /* Configure. */
        source.Initialize(config.Source);

        /* Use. */
        Source = source;
    }
}
