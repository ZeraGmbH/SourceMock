using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Exceptions;
using SourceApi.Actions.SimulatedSource;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Models.Dosage;
using ZERA.WebSam.Shared.Provider;

namespace SourceApi.Actions;

/// <summary>
/// Implementation of a source which is not configured and can therefore not be used.
/// </summary>
public class UnavailableSource(IDosage? dosage = null) : IACSourceMock, IDCSourceMock
{
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(false);

    public Task CancelDosageAsync(IInterfaceLogger logger)
        => dosage?.CancelDosageAsync(logger) ?? throw new SourceNotReadyException();

    public Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
        => dosage?.CurrentSwitchedOffForDosageAsync(logger) ?? throw new SourceNotReadyException();

    public Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger interfaceLogger) => throw new SourceNotReadyException();

    public Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) => Task.FromResult<SourceCapabilities>(null!);

    public Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger interfaceLogger) => throw new SourceNotReadyException();

    public Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
        => dosage?.GetDosageProgressAsync(logger, meterConstant) ?? throw new SourceNotReadyException();

    public Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
        => dosage?.SetDosageEnergyAsync(logger, value, meterConstant) ?? throw new SourceNotReadyException();

    public Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
        => dosage?.SetDosageModeAsync(logger, on) ?? throw new SourceNotReadyException();

    public Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint) => throw new SourceNotReadyException();

    public Task StartDosageAsync(IInterfaceLogger logger)
        => dosage?.StartDosageAsync(logger) ?? throw new SourceNotReadyException();

    public Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger) => throw new SourceNotReadyException();

    /// <inheritdoc/>
    public Task StartEnergyAsync(IInterfaceLogger logger)
        => dosage?.StartEnergyAsync(logger) ?? throw new SourceNotReadyException();

    /// <inheritdoc/>
    public Task StopEnergyAsync(IInterfaceLogger logger)
        => dosage?.StopEnergyAsync(logger) ?? throw new SourceNotReadyException();

    /// <inheritdoc/>
    public Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger)
        => dosage?.GetEnergyAsync(logger) ?? throw new SourceNotReadyException();
}