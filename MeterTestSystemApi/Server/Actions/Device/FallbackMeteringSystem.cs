using MeterTestSystemApi.Models;
using RefMeterApi.Actions;
using SourceApi.Actions;
using ZERA.WebSam.Shared.Provider;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Fallback implementation of a meter test system.
/// </summary>
public class FallbackMeteringSystem : IMeterTestSystem
{
    /// <inheritdoc/>
    public Task<AmplifiersAndReferenceMeter> GetAmplifiersAndReferenceMeterAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool HasSource { get; } = false;

    /// <inheritdoc/>
    public bool HasDosage { get; } = false;

    /// <inheritdoc/>
    public ISource Source { get; } = new UnavailableSource();

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; } = new UnavailableReferenceMeter();

    /// <inheritdoc/>
    public IErrorCalculator[] ErrorCalculators => [];

    /// <inheritdoc/>
#pragma warning disable CS0414
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) =>
        Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditionsAsync(IInterfaceLogger logger) =>
        Task.FromResult(new ErrorConditions());

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger) =>
        Task.FromResult(new MeterTestSystemFirmwareVersion
        {
            ModelName = "FallbackMeterTestSystem",
            Version = "0.1"
        });

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeterAsync(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings)
    {
        /* The fallback do not support amplifier configurations. */
        throw new InvalidOperationException();
    }
}
