using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

/// <summary>
/// Probing plan.
/// </summary>
public partial class ConfigurationProbePlan(IProbingOperationStore store) : IConfigurationProbePlan
{
    private List<Probe> _probes { get; set; } = [];

    private ProbingOperation _operation = null!;

    private ProbeConfigurationResult _result => _operation.Result;

    private ProbeConfigurationRequest _request => _operation.Request;

    /// <inheritdoc/>
    public Task<ProbingOperation> ConfigureProbeAsync() => Task.Run(() =>
    {
        _operation =
            store
                .Query()
                .Where(o => o.Finished == null)
                .OrderByDescending(o => o.Created)
                .FirstOrDefault()
            ?? throw new InvalidOperationException("no active probe request");

        _operation.Result = new();

        _probes.Clear();

        AddTcpIpProbes();
        AddSerialProbes();
        AddHIDProbes();

        for (var i = 0; i < _request.Configuration.TestPositions.Count; i++)
            _result.Configuration.TestPositions.Add(new());

        return _operation;
    });

    /// <inheritdoc/>
    public async Task FinishProbeAsync(CancellationToken cancel)
    {
        foreach (var probe in _probes)
            _result.Log.Add($"{probe}: {probe.Result}");

        _operation.Finished = DateTime.UtcNow;

        await store.UpdateAsync(_operation);
    }
}
