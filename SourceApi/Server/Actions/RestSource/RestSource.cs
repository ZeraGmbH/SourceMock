using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.RestSource;

/// <summary>
/// Source connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpSource">Source connection to use.</param>
/// <param name="httpDosage">Dosage connection to use.</param>
/// <param name="logger">Logging helper.</param>
public class RestSource(ILoggingHttpClient httpSource, ILoggingHttpClient httpDosage, ILogger<RestSource> logger) : IRestSource
{
    private bool _initialized = false;

    private Uri _sourceUri = null!;

    private Uri? _dosageUri = null;

    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger)
    {
        /* Not yet initialized. */
        if (!_initialized) return false;

        try
        {
            var available = httpSource.GetAsync<bool>(interfaceLogger, new Uri(_sourceUri, "Available"));

            available.Wait();

            return available.Result;
        }
        catch (Exception e)
        {
            /* Just report the error. */
            logger.LogError("Unable to connect to remote source API: {Exception}",
                e is AggregateException ae
                ? ae.InnerExceptions[0].Message
                : e.Message);

            return false;
        }
    }

    /// <inheritdoc/>
    public async Task CancelDosage(IInterfaceLogger logger)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, "Cancel"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger) =>
        (_dosageUri == null)
            ? Task.FromResult(false)
            : httpDosage.GetAsync<bool>(logger, new Uri(_dosageUri, "IsDosageCurrentOff"));

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo(IInterfaceLogger interfaceLogger)
    {
        var req = httpSource.GetAsync<LoadpointInfo>(interfaceLogger, new Uri(_sourceUri, "LoadpointInfo"));

        req.Wait();

        return req.Result;
    }

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger) =>
        httpSource.GetAsync<SourceCapabilities>(interfaceLogger, new Uri(_sourceUri, "Capabilities"));

    /// <inheritdoc/>
    public TargetLoadpoint? GetCurrentLoadpoint(IInterfaceLogger interfaceLogger)
    {
        var req = httpSource.GetAsync<TargetLoadpoint?>(interfaceLogger, new Uri(_sourceUri, "Loadpoint"));

        req.Wait();

        return req.Result;
    }

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant) =>
        (_dosageUri == null)
            ? throw new NotImplementedException("Dosage")
            : httpDosage.GetAsync<DosageProgress>(logger, new Uri(_dosageUri, $"Progress?meterConstant={JsonSerializer.Serialize(meterConstant, LibUtils.JsonSettings)}"));

    /// <inheritdoc/>
    public void Initialize(RestConfiguration? sourceEndpoint, RestConfiguration? dosageEndpoint)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _sourceUri = null!;
        _dosageUri = null;

        /* Validate. */
        if (string.IsNullOrEmpty(sourceEndpoint?.Endpoint)) throw new InvalidOperationException("no source connection configured");

        /* Configure connection for logging. */
        httpSource.LogConnection = new()
        {
            Endpoint = sourceEndpoint.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Http,
            WebSamType = InterfaceLogSourceTypes.Source,
        };

        _sourceUri = new Uri(sourceEndpoint.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_sourceUri.UserInfo))
            httpSource.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_sourceUri.UserInfo)));

        /* Dosage is optional. */
        if (!string.IsNullOrEmpty(dosageEndpoint?.Endpoint))
        {
            /* Configure connection for logging. */
            httpDosage.LogConnection = new()
            {
                Endpoint = dosageEndpoint.Endpoint,
                Protocol = InterfaceLogProtocolTypes.Http,
                WebSamType = InterfaceLogSourceTypes.Source,
            };

            _dosageUri = new Uri(dosageEndpoint.Endpoint.TrimEnd('/') + "/");

            if (!string.IsNullOrEmpty(_dosageUri.UserInfo))
                httpDosage.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_dosageUri.UserInfo)));
        }

        /* Did it. */
        _initialized = true;
    }

    /// <inheritdoc/>
    public async Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PutAsync(logger, new Uri(_dosageUri, $"Energy?energy={JsonSerializer.Serialize(value, LibUtils.JsonSettings)}&meterConstant={JsonSerializer.Serialize(meterConstant, LibUtils.JsonSettings)}"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task SetDosageMode(IInterfaceLogger logger, bool on)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, $"DOSMode?on={JsonSerializer.Serialize(on, LibUtils.JsonSettings)}"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        var res = await httpSource.PutAsync(logger, new Uri(_sourceUri, "Loadpoint"), loadpoint);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }

    /// <inheritdoc/>
    public async Task StartDosage(IInterfaceLogger logger)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PostAsync(logger, new Uri(_dosageUri, "Start"));

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger)
    {
        var res = await httpSource.PostAsync(logger, new Uri(_sourceUri, "TurnOff"));

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }
}
