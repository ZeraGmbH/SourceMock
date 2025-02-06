using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MockDevices.Source;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Dosage;
using ZERA.WebSam.Shared.Models.Logging;

namespace RestDevices.Dosage;

/// <summary>
/// Dosage algorithm connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpDosage">Dosage connection to use.</param>
public class RestDosage(ILoggingHttpClient httpDosage) : IRestDosage, IDosageMock, IDisposable
{
    private bool _initialized = false;

    private Uri _dosageUri = null!;

    private bool _disposed = false;

    /// <inheritdoc/>
    public async Task CancelDosageAsync(IInterfaceLogger logger)
    {
        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, "Cancel"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
        => httpDosage.GetAsync<bool>(logger, new Uri(_dosageUri, "IsDosageCurrentOff"));

    /// <inheritdoc/>
    public void Dispose() => _disposed = true;

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
        => httpDosage.GetAsync<DosageProgress>(logger, new Uri(_dosageUri, $"Progress?meterConstant={JsonSerializer.Serialize(meterConstant, LibUtils.JsonSettings)}"));

    /// <inheritdoc/>
    public void Initialize(RestConfiguration? endpoint)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _dosageUri = null!;

        /* Validate. */
        if (string.IsNullOrEmpty(endpoint?.Endpoint)) throw new InvalidOperationException("no dosage connection configured");

        /* Configure connection for logging. */
        httpDosage.LogConnection = new()
        {
            Endpoint = endpoint.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Http,
            WebSamType = InterfaceLogSourceTypes.Dosage,
        };

        _dosageUri = new Uri(endpoint.Endpoint.TrimEnd('/') + "/");

        if (!string.IsNullOrEmpty(_dosageUri.UserInfo))
            httpDosage.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_dosageUri.UserInfo)));

        /* Did it. */
        _initialized = true;
    }

    /// <inheritdoc/>
    public Task NoSourceAsync(IInterfaceLogger interfaceLogger)
    {
        ThreadPool.QueueUserWorkItem((state) =>
        {
            for (; !_disposed; Thread.Sleep(1000))
                try
                {
                    httpDosage.PostAsync(interfaceLogger, new Uri(_dosageUri, "NoSource")).Wait();

                    break;
                }
                catch (Exception e)
                {
                    if (e is AggregateException aggr)
                        e = aggr.InnerExceptions.FirstOrDefault() ?? e;

                    if (e is not HttpRequestException req || req.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Dosage endpoint does not support mock interface: {e.Message}");

                        break;
                    }
                }
        });

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        var res = await httpDosage.PutAsync(logger, new Uri(_dosageUri, $"Energy?energy={JsonSerializer.Serialize(value, LibUtils.JsonSettings)}&meterConstant={JsonSerializer.Serialize(meterConstant, LibUtils.JsonSettings)}"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
    {
        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, $"DOSMode?on={JsonSerializer.Serialize(on, LibUtils.JsonSettings)}"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task StartDosageAsync(IInterfaceLogger logger)
    {
        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, "Start"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task StopEnergyAsync(IInterfaceLogger logger)
    {
        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, "StopEnergy"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task StartEnergyAsync(IInterfaceLogger logger)
    {
        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, "StartEnergy"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger)
        => httpDosage.GetAsync<ActiveEnergy>(logger, new Uri(_dosageUri, "Energy"));
}
