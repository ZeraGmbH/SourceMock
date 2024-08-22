using Microsoft.Extensions.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace SourceApi.Actions.RestSource;

/// <summary>
/// Source connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpSource">Source connection to use.</param>
/// <param name="logger">Logging helper.</param>
public class RestSource(ILoggingHttpClient httpSource, ILogger<RestSource> logger) : IRestSource
{
    private bool _initialized = false;

    private Uri _sourceUri = null!;

    private IDosage? _dosage = null;

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
    public Task CancelDosage(IInterfaceLogger logger)
        => _dosage == null ? throw new NotImplementedException("Dosage") : _dosage.CancelDosage(logger);

    /// <inheritdoc/>
    public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger)
        => _dosage == null ? Task.FromResult(false) : _dosage.CurrentSwitchedOffForDosage(logger);

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
    public Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant)
        => _dosage == null ? throw new NotImplementedException("Dosage") : _dosage.GetDosageProgress(logger, meterConstant);

    /// <inheritdoc/>
    public void Initialize(RestConfiguration? sourceEndpoint, IDosage? dosage)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _sourceUri = null!;
        _dosage = null;

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
        _dosage = dosage;

        /* Did it. */
        _initialized = true;
    }

    /// <inheritdoc/>
    public Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
        => _dosage == null ? throw new NotImplementedException("Dosage") : _dosage.SetDosageEnergy(logger, value, meterConstant);

    /// <inheritdoc/>
    public Task SetDosageMode(IInterfaceLogger logger, bool on)
        => _dosage == null ? throw new NotImplementedException("Dosage") : _dosage.SetDosageMode(logger, on);

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        var res = await httpSource.PutAsync(logger, new Uri(_sourceUri, "Loadpoint"), loadpoint);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }

    /// <inheritdoc/>
    public Task StartDosage(IInterfaceLogger logger)
        => _dosage == null ? throw new NotImplementedException("Dosage") : _dosage.StartDosage(logger);

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger)
    {
        var res = await httpSource.PostAsync(logger, new Uri(_sourceUri, "TurnOff"));

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }
}
