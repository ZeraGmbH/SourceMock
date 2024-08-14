using ErrorCalculatorApi.Actions;
using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models;
using RefMeterApi.Actions.Device;
using SourceApi.Actions.Source;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Fallback implementation of a meter test system.
/// </summary>
public class FallbackMeteringSystem : IMeterTestSystem
{
    /// <inheritdoc/>
    public AmplifiersAndReferenceMeter GetAmplifiersAndReferenceMeter(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public ISource Source { get; } = new UnavailableSource();

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
    public Task<MeterTestSystemCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger) =>
        Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditions(IInterfaceLogger logger) =>
        Task.FromResult(new ErrorConditions());

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) =>
        Task.FromResult(new MeterTestSystemFirmwareVersion
        {
            ModelName = "Unknown",
            Version = "0.0"
        });

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings)
    {
        /* The fallback do not support amplifier configurations. */
        throw new InvalidOperationException();
    }
}
