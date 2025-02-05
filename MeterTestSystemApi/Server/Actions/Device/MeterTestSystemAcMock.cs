using ErrorCalculatorApi.Actions.Device;
using RefMeterApi.Actions.Device;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Actions;
using ZERA.WebSam.Shared.Provider;
using ZERA.WebSam.Shared.Models.MeterTestSystem;

namespace MeterTestSystemApi;

/// <summary>
/// 
/// </summary>
/// <param name="source"></param>
/// <param name="refMeter"></param>
/// <param name="errorCalculatorMock"></param>
public class MeterTestSystemAcMock(IACSourceMock source, IMockRefMeter refMeter, IErrorCalculatorMock errorCalculatorMock) : IMeterTestSystem
{
    /// <inheritdoc/>
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;

    /// <inheritdoc/>
    public Task<AmplifiersAndReferenceMeter> GetAmplifiersAndReferenceMeterAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool HasSource { get; private set; } = true;

    /// <inheritdoc/>
    public bool HasDosage { get; private set; } = true;

    /// <inheritdoc/>
    public ISource Source { get; private set; } = source;

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; } = refMeter;

    private readonly List<IErrorCalculator> _errorCalculators = [errorCalculatorMock];

    /// <inheritdoc/>
    public IErrorCalculator[] ErrorCalculators => [.. _errorCalculators];

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) => Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger) =>
        Task.FromResult(new MeterTestSystemFirmwareVersion
        {
            ModelName = "DeviceMock",
            Version = "1.0"
        });

    /// <summary>
    /// 
    /// </summary>
    public void NoSource()
    {
        Source = new UnavailableSource();
        HasDosage = false;
        HasSource = false;
    }

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeterAsync(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditionsAsync(IInterfaceLogger logger)
    {
        var errors = new ErrorConditions();

        errors.Amplifiers[Amplifiers.Current2].Overload = DateTime.Now.Minute % 2 == 0;
        errors.HasFuseError = DateTime.Now.Minute % 3 == 0;

        ErrorConditionsChanged?.Invoke(errors);

        return Task.FromResult(errors);
    }
}
