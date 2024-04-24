using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.RestSource;

/// <summary>
/// Source connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpSource">Source connection to use.</param>
/// <param name="httpDosage">Dosage connection to use.</param>
/// <param name="logger">Logging helper.</param>
public class RestSource(HttpClient httpSource, HttpClient httpDosage, ILogger<RestSource> logger) : IRestSource
{
    private bool _initialized = false;

    private Uri _sourceUri = null!;

    private Uri? _dosageUri = null;

    /// <inheritdoc/>
    public bool Available
    {
        get
        {
            /* Not yet initialized. */
            if (!_initialized) return false;

            try
            {
                var available = httpSource.GetAsync(new Uri(_sourceUri, "Available")).GetJsonResponse<bool>();

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
    }

    /// <inheritdoc/>
    public async Task CancelDosage(IInterfaceLogger logger)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PostAsync(new Uri(_dosageUri, "Cancel"), null);

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger) =>
        (_dosageUri == null)
            ? Task.FromResult(false)
            : httpDosage.GetAsync(new Uri(_dosageUri, "IsDosageCurrentOff")).GetJsonResponse<bool>();

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo()
    {
        var req = httpSource.GetAsync(new Uri(_sourceUri, "LoadpointInfo")).GetJsonResponse<LoadpointInfo>();

        req.Wait();

        return req.Result;
    }

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities() =>
        httpSource.GetAsync(new Uri(_sourceUri, "Capabilities")).GetJsonResponse<SourceCapabilities>();

    /// <inheritdoc/>
    public TargetLoadpoint? GetCurrentLoadpoint()
    {
        var req = httpSource.GetAsync(new Uri(_sourceUri, "Loadpoint")).GetJsonResponse<TargetLoadpoint?>();

        req.Wait();

        return req.Result;
    }

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, double meterConstant) =>
        (_dosageUri == null)
            ? throw new NotImplementedException("Dosage")
            : httpDosage.GetAsync(new Uri(_dosageUri, $"Progress?meterConstant={JsonConvert.SerializeObject(meterConstant)}")).GetJsonResponse<DosageProgress>();

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

        _sourceUri = new Uri(sourceEndpoint.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_sourceUri.UserInfo))
            httpSource.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_sourceUri.UserInfo)));

        /* Dosage is optional. */
        if (!string.IsNullOrEmpty(dosageEndpoint?.Endpoint))
        {
            _dosageUri = new Uri(dosageEndpoint.Endpoint.TrimEnd('/') + "/");

            if (!string.IsNullOrEmpty(_dosageUri.UserInfo))
                httpDosage.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_dosageUri.UserInfo)));
        }

        /* Did it. */
        _initialized = true;
    }

    /// <inheritdoc/>
    public async Task SetDosageEnergy(IInterfaceLogger logger, double value, double meterConstant)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PutAsync(new Uri(_dosageUri, $"Energy?energy={JsonConvert.SerializeObject(value)}&meterConstant={JsonConvert.SerializeObject(meterConstant)}"), null);

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task SetDosageMode(IInterfaceLogger logger, bool on)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PostAsync(new Uri(_dosageUri, $"DOSMode?on={JsonConvert.SerializeObject(on)}"), null);

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        var res = await httpSource.PutAsJsonAsync(new Uri(_sourceUri, "Loadpoint"), loadpoint);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }

    /// <inheritdoc/>
    public async Task StartDosage(IInterfaceLogger logger)
    {
        if (_dosageUri == null) throw new NotImplementedException("Dosage");

        var res = await httpDosage.PostAsync(new Uri(_dosageUri, "Start"), null);

        if (res.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger)
    {
        var res = await httpSource.PostAsync(new Uri(_sourceUri, "TurnOff"), null);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }
}
