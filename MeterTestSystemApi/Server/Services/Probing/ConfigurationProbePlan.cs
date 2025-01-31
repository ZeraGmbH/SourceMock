using BarcodeApi.Actions;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MeterTestSystemApi.Services.Probing;

/// <summary>
/// Probing plan.
/// </summary>
public partial class ConfigurationProbePlan : IConfigurationProbePlan
{
    private List<Probe> _probes { get; set; } = [];

    private ProbingOperation _operation = null!;

    private ProbeConfigurationResult _result => _operation.Result;

    private ProbeConfigurationRequest _request => _operation.Request;

    private readonly Dictionary<Type, Func<Probe, Task>> _handlers;

    private readonly IProbingOperationStore _store;

    private readonly IServiceProvider _services;

    private readonly ILogger<ConfigurationProbePlan> _logger;

    private readonly bool _useLocalhost;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="store"></param>
    /// <param name="logger"></param>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public ConfigurationProbePlan(
        IProbingOperationStore store, ILogger<ConfigurationProbePlan> logger, IServiceProvider services, IConfiguration configuration)
    {
        _useLocalhost = configuration.GetValue<bool>("UseLocalhostForIPProbing");

        _handlers = new() {
            { typeof(SerialProbe), p => ProbeSerialAsync((SerialProbe)p) },
            { typeof(IPProbe), p => ProbeTcpIpAsync((IPProbe)p) },
            { typeof(HIDProbe), p => ProbeHidAsync((HIDProbe)p) },
        };

        _logger = logger;
        _services = services;
        _store = store;
    }

    /// <inheritdoc/>
    public Task<ProbingOperation> ConfigureProbeAsync() => Task.Run(() =>
    {
        _operation =
            _store
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
        {
            if (_handlers.TryGetValue(probe.GetType(), out var handlerAsync))
                await handlerAsync(probe);

            _result.Log.Add($"{probe}: {probe.Result}");
        }

        _operation.Finished = DateTime.UtcNow;

        await _store.UpdateAsync(_operation);
    }
}
