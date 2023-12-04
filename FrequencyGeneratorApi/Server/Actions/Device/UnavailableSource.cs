using SourceApi.Actions.Source;
using SourceApi.Model;

namespace MeteringSystemApi;

internal class UnavailableSource : ISource
{
    public bool Available => false;

    public Task CancelDosage()
    {
        throw new NotImplementedException();
    }

    public LoadpointInfo GetActiveLoadpointInfo()
    {
        throw new NotImplementedException();
    }

    public Task<SourceCapabilities> GetCapabilities() => Task.FromResult<SourceCapabilities>(null!);

    public Loadpoint? GetCurrentLoadpoint()
    {
        throw new NotImplementedException();
    }

    public Task<DosageProgress> GetDosageProgress()
    {
        throw new NotImplementedException();
    }

    public Task SetDosageEnergy(double value)
    {
        throw new NotImplementedException();
    }

    public Task SetDosageMode(bool on)
    {
        throw new NotImplementedException();
    }

    public Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        throw new NotImplementedException();
    }

    public Task StartDosage()
    {
        throw new NotImplementedException();
    }

    public Task<SourceResult> TurnOff()
    {
        throw new NotImplementedException();
    }
}
