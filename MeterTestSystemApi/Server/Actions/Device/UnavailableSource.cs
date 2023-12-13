using SourceApi.Actions.Source;
using SourceApi.Exceptions;
using SourceApi.Model;

namespace MeterTestSystemApi;

internal class UnavailableSource : ISource
{
    public bool Available => false;

    public Task CancelDosage()
    {
        throw new SourceNotReadyException();
    }

    public Task<bool> CurrentSwitchedOffForDosage()
    {
        throw new NotImplementedException();
    }

    public LoadpointInfo GetActiveLoadpointInfo()
    {
        throw new SourceNotReadyException();
    }

    public Task<SourceCapabilities> GetCapabilities() => Task.FromResult<SourceCapabilities>(null!);

    public Loadpoint? GetCurrentLoadpoint()
    {
        throw new SourceNotReadyException();
    }

    public Task<DosageProgress> GetDosageProgress()
    {
        throw new SourceNotReadyException();
    }

    public Task SetDosageEnergy(double value)
    {
        throw new SourceNotReadyException();
    }

    public Task SetDosageMode(bool on)
    {
        throw new SourceNotReadyException();
    }

    public Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        throw new SourceNotReadyException();
    }

    public Task StartDosage()
    {
        throw new SourceNotReadyException();
    }

    public Task<SourceResult> TurnOff()
    {
        throw new SourceNotReadyException();
    }
}
