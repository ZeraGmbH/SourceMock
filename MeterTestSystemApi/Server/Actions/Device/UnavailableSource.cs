using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Exceptions;
using SourceApi.Model;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implementation of a source which is not configured and can therefore not be used.
/// </summary>
internal class UnavailableSource : ISource
{
    public bool Available => false;

    public Task CancelDosage(IInterfaceLogger logger) => throw new SourceNotReadyException();

    public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger) => throw new SourceNotReadyException();

    public LoadpointInfo GetActiveLoadpointInfo() => throw new SourceNotReadyException();

    public Task<SourceCapabilities> GetCapabilities() => Task.FromResult<SourceCapabilities>(null!);

    public TargetLoadpoint? GetCurrentLoadpoint() => throw new SourceNotReadyException();

    public Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, double meterConstant) => throw new SourceNotReadyException();

    public Task SetDosageEnergy(IInterfaceLogger logger, double value, double meterConstant) => throw new SourceNotReadyException();

    public Task SetDosageMode(IInterfaceLogger logger, bool on) => throw new SourceNotReadyException();

    public Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint) => throw new SourceNotReadyException();

    public Task StartDosage(IInterfaceLogger logger) => throw new SourceNotReadyException();

    public Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger) => throw new SourceNotReadyException();
}
