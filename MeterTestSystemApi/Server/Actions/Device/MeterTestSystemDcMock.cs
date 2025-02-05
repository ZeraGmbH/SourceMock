using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using RefMeterApi.Actions.Device;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.SimulatedSource;
using SourceApi.Actions;
using ZERA.WebSam.Shared.Provider;

namespace MeterTestSystemApi;

/// <summary>
/// 
/// </summary>
/// <param name="source"></param>
/// <param name="refMeter"></param>
/// <param name="errorCalculatorMock"></param>
public class MeterTestSystemDcMock(IDCSourceMock source, IDCRefMeterMock refMeter, IErrorCalculatorMock errorCalculatorMock) : IMeterTestSystem
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
            ModelName = "DcDeviceMock",
            Version = "1.0"
        });

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeterAsync(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditionsAsync(IInterfaceLogger logger)
    {
        var errors = new ErrorConditions { HasFuseError = DateTime.Now.Minute % 2 == 0 };

        ErrorConditionsChanged?.Invoke(errors);

        return Task.FromResult(errors);
    }

    /// <summary>
    /// 
    /// </summary>
    public void NoSource()
    {
        Source = new UnavailableSource();
        HasDosage = false;
        HasSource = false;
    }
}
