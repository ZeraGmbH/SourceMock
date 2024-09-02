using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Exceptions;
using SourceApi.Model;
using SourceApi.Actions.SimulatedSource;

namespace SourceApi.Actions;

/// <summary>
/// Implementation of a source which is not configured and can therefore not be used.
/// </summary>
public class UnavailableSource(IDosage? dosage = null) : IACSourceMock, IDCSourceMock
{
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(false);

    public Task CancelDosageAsync(IInterfaceLogger logger)
        => dosage == null ? throw new SourceNotReadyException() : dosage.CancelDosageAsync(logger);

    public Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
        => dosage == null ? throw new SourceNotReadyException() : dosage.CurrentSwitchedOffForDosageAsync(logger);

    public Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger interfaceLogger) => throw new SourceNotReadyException();

    public Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) => Task.FromResult<SourceCapabilities>(null!);

    public Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger interfaceLogger) => throw new SourceNotReadyException();

    public Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
        => dosage == null ? throw new SourceNotReadyException() : dosage.GetDosageProgressAsync(logger, meterConstant);

    public Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
        => dosage == null ? throw new SourceNotReadyException() : dosage.SetDosageEnergyAsync(logger, value, meterConstant);

    public Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
        => dosage == null ? throw new SourceNotReadyException() : dosage.SetDosageModeAsync(logger, on);

    public Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint) => throw new SourceNotReadyException();

    public Task StartDosageAsync(IInterfaceLogger logger)
        => dosage == null ? throw new SourceNotReadyException() : dosage.StartDosageAsync(logger);

    public Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger) => throw new SourceNotReadyException();
}