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

    public Task CancelDosage() => throw new SourceNotReadyException();

    public Task<bool> CurrentSwitchedOffForDosage() => throw new SourceNotReadyException();

    public LoadpointInfo GetActiveLoadpointInfo() => throw new SourceNotReadyException();

    public Task<SourceCapabilities> GetCapabilities() => Task.FromResult<SourceCapabilities>(null!);

    public TargetLoadpoint? GetCurrentLoadpoint() => throw new SourceNotReadyException();

    public Task<DosageProgress> GetDosageProgress() => throw new SourceNotReadyException();

    public Task SetDosageEnergy(double value) => throw new SourceNotReadyException();

    public Task SetDosageMode(bool on) => throw new SourceNotReadyException();

    public Task<SourceApiErrorCodes> SetLoadpoint(TargetLoadpoint loadpoint) => throw new SourceNotReadyException();

    public Task StartDosage() => throw new SourceNotReadyException();

    public Task<SourceApiErrorCodes> TurnOff() => throw new SourceNotReadyException();
}
