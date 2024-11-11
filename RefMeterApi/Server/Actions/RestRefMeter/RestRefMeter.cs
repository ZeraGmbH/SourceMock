using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Actions.RestSource;

/// <summary>
/// Reference meter connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpClient">Reference meter connection to use.</param>
/// <param name="logger">Logging helper.</param>
public class RestRefMeter(ILoggingHttpClient httpClient, ILogger<RestRefMeter> logger) : IRestRefMeter
{
    private bool _initialized = false;

    private Uri _baseUri = null!;

    /// <inheritdoc/>
    public async Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger)
    {
        /* Not yet initialized. */
        if (!_initialized) return false;

        try
        {
            return await httpClient.GetAsync<bool>(interfaceLogger, new Uri(_baseUri, "Available"));
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
    public Task<MeasurementModes?> GetActualMeasurementModeAsync(IInterfaceLogger logger) =>
        httpClient.GetAsync<MeasurementModes?>(logger, new Uri(_baseUri, "MeasurementMode"));

    /// <inheritdoc/>
    public async Task<MeterConstant> GetMeterConstantAsync(IInterfaceLogger logger) =>
        new(await httpClient.GetAsync<double>(logger, new Uri(_baseUri, "MeterConstant")));

    /// <inheritdoc/>
    public Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveCurrentPhase = -1) =>
        httpClient.GetAsync<MeasuredLoadpoint>(logger, new Uri(_baseUri, $"ActualValues?firstActiveCurrentPhase={JsonSerializer.Serialize(firstActiveCurrentPhase, LibUtils.JsonSettings)}"));

    /// <inheritdoc/>
    public Task<MeasuredLoadpoint> GetActualValuesUncachedAsync(IInterfaceLogger logger, int firstActiveCurrentPhase = -1, bool singlePhase = false) =>
        GetActualValuesAsync(logger, firstActiveCurrentPhase);

    /// <inheritdoc/>
    public Task<MeasurementModes[]> GetMeasurementModesAsync(IInterfaceLogger logger) =>
        httpClient.GetAsync<MeasurementModes[]>(logger, new Uri(_baseUri, "MeasurementModes"));

    /// <inheritdoc/>
    public Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger) =>
        httpClient.GetAsync<ReferenceMeterInformation>(logger, new Uri(_baseUri, "Version"));

    /// <inheritdoc/>
    public void Initialize(RestConfiguration? endpoint)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _baseUri = null!;

        /* Validate. */
        if (string.IsNullOrEmpty(endpoint?.Endpoint)) throw new InvalidOperationException("no reference meter connection configured");

        /* Configure connection for logging. */
        httpClient.LogConnection = new()
        {
            Endpoint = endpoint.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Http,
            WebSamType = InterfaceLogSourceTypes.ReferenceMeter,
        };

        _baseUri = new Uri(endpoint.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_baseUri.UserInfo))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_baseUri.UserInfo)));

        /* Did it. */
        _initialized = true;
    }

    /// <inheritdoc/>
    public Task SetActualMeasurementModeAsync(IInterfaceLogger logger, MeasurementModes mode) =>
        httpClient.PutAsync(logger, new Uri(_baseUri, $"MeasurementMode?mode={mode}"));

    /// <inheritdoc/>
    public Task<Voltage[]> GetVoltageRangesAsync(IInterfaceLogger logger)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<Current[]> GetCurrentRangesAsync(IInterfaceLogger logger)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetVoltageRangeAsync(IInterfaceLogger logger, Voltage voltage)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetCurrentRangeAsync(IInterfaceLogger logger, Current current)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetAutomaticAsync(IInterfaceLogger logger, bool voltageRanges = true, bool currentRanges = true, bool pll = true)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SelectPllChannelAsync(IInterfaceLogger logger, PllChannel pll)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<RefMeterStatus> GetRefMeterStatusAsync(IInterfaceLogger logger)
    {
        throw new NotImplementedException();
    }
}
