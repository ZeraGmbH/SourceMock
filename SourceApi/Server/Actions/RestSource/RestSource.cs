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
    public async Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger)
    {
        /* Not yet initialized. */
        if (!_initialized) return false;

        try
        {
            return await httpSource.GetAsync<bool>(interfaceLogger, new Uri(_sourceUri, "Available"));
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
    public Task CancelDosageAsync(IInterfaceLogger logger)
        => _dosage?.CancelDosageAsync(logger) ?? throw new NotImplementedException("Dosage");

    /// <inheritdoc/>
    public Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
        => _dosage?.CurrentSwitchedOffForDosageAsync(logger) ?? Task.FromResult(false);

    /// <inheritdoc/>
    public Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger interfaceLogger)
        => httpSource.GetAsync<LoadpointInfo>(interfaceLogger, new Uri(_sourceUri, "LoadpointInfo"));

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) =>
        httpSource.GetAsync<SourceCapabilities>(interfaceLogger, new Uri(_sourceUri, "Capabilities"));

    /// <inheritdoc/>
    public Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger interfaceLogger)
        => httpSource.GetAsync<TargetLoadpoint?>(interfaceLogger, new Uri(_sourceUri, "Loadpoint"));

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
        => _dosage?.GetDosageProgressAsync(logger, meterConstant) ?? throw new NotImplementedException("Dosage");

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
    public Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
        => _dosage?.SetDosageEnergyAsync(logger, value, meterConstant) ?? throw new NotImplementedException("Dosage");

    /// <inheritdoc/>
    public Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
        => _dosage?.SetDosageModeAsync(logger, on) ?? throw new NotImplementedException("Dosage");

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        var res = await httpSource.PutAsync(logger, new Uri(_sourceUri, "Loadpoint"), loadpoint);

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }

    /// <inheritdoc/>
    public Task StartDosageAsync(IInterfaceLogger logger)
        => _dosage?.StartDosageAsync(logger) ?? throw new NotImplementedException("Dosage");

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger)
    {
        var res = await httpSource.PostAsync(logger, new Uri(_sourceUri, "TurnOff"));

        return res.StatusCode == HttpStatusCode.OK ? SourceApiErrorCodes.SUCCESS : SourceApiErrorCodes.LOADPOINT_NOT_SET;
    }

    /// <inheritdoc/>
    public Task StartEnergyAsync(IInterfaceLogger logger)
        => _dosage?.StartEnergyAsync(logger) ?? throw new NotImplementedException("Dosage");

    /// <inheritdoc/>
    public Task StopEnergyAsync(IInterfaceLogger logger)
        => _dosage?.StopEnergyAsync(logger) ?? throw new NotImplementedException("Dosage");

    /// <inheritdoc/>
    public Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger)
        => _dosage?.GetEnergyAsync(logger) ?? throw new NotImplementedException("Dosage");
}
