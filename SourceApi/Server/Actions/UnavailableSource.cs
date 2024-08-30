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
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => false;

    public Task CancelDosage(IInterfaceLogger logger)
        => dosage == null ? throw new SourceNotReadyException() : dosage.CancelDosage(logger);

    public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger)
        => dosage == null ? throw new SourceNotReadyException() : dosage.CurrentSwitchedOffForDosage(logger);

    public LoadpointInfo GetActiveLoadpointInfo(IInterfaceLogger interfaceLogger) => throw new SourceNotReadyException();

    public Task<SourceCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger) => Task.FromResult<SourceCapabilities>(null!);

    public TargetLoadpoint? GetCurrentLoadpoint(IInterfaceLogger interfaceLogger) => throw new SourceNotReadyException();

    public Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant)
        => dosage == null ? throw new SourceNotReadyException() : dosage.GetDosageProgress(logger, meterConstant);

    public Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
        => dosage == null ? throw new SourceNotReadyException() : dosage.SetDosageEnergy(logger, value, meterConstant);

    public Task SetDosageMode(IInterfaceLogger logger, bool on)
        => dosage == null ? throw new SourceNotReadyException() : dosage.SetDosageMode(logger, on);

    public Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint) => throw new SourceNotReadyException();

    public Task StartDosage(IInterfaceLogger logger)
        => dosage == null ? throw new SourceNotReadyException() : dosage.StartDosage(logger);

    public Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger) => throw new SourceNotReadyException();
}