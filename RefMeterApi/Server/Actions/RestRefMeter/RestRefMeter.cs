using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefMeterApi.Models;
using SharedLibrary;
using SharedLibrary.Models;
using SharedLibrary.Models.Logging;

namespace RefMeterApi.Actions.RestSource;

/// <summary>
/// Reference meter connected to a HTTP/REST web service.
/// </summary>
/// <param name="httpClient">Reference meter connection to use.</param>
/// <param name="logger">Logging helper.</param>
public class RestRefMeter(HttpClient httpClient, ILogger<RestRefMeter> logger) : IRestRefMeter
{
    private bool _initialized = false;

    private Uri _baseUri = null!;

    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger)
    {
        /* Not yet initialized. */
        if (!_initialized) return false;

        try
        {
            var available = httpClient.GetAsync(new Uri(_baseUri, "Available")).GetJsonResponse<bool>();

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
    public Task<MeasurementModes?> GetActualMeasurementMode(IInterfaceLogger logger) =>
        httpClient.GetAsync(new Uri(_baseUri, "MeasurementMode")).GetJsonResponse<MeasurementModes?>();

    /// <inheritdoc/>
    public Task<double> GetMeterConstant(IInterfaceLogger logger) =>
        httpClient.GetAsync(new Uri(_baseUri, "MeterConstant")).GetJsonResponse<double>();

    /// <inheritdoc/>
    public Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveCurrentPhase = -1) =>
        httpClient.GetAsync($"{new Uri(_baseUri, "ActualValues")}?firstActiveCurrentPhase={JsonConvert.SerializeObject(firstActiveCurrentPhase)}").GetJsonResponse<MeasuredLoadpoint>();

    /// <inheritdoc/>
    public Task<MeasurementModes[]> GetMeasurementModes(IInterfaceLogger logger) =>
        httpClient.GetAsync(new Uri(_baseUri, "MeasurementModes")).GetJsonResponse<MeasurementModes[]>();

    /// <inheritdoc/>
    public void Initialize(RestConfiguration? endpoint)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _baseUri = null!;

        /* Validate. */
        if (string.IsNullOrEmpty(endpoint?.Endpoint)) throw new InvalidOperationException("no reference meter connection configured");

        _baseUri = new Uri(endpoint.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_baseUri.UserInfo))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_baseUri.UserInfo)));

        /* Did it. */
        _initialized = true;
    }

    /// <inheritdoc/>
    public Task SetActualMeasurementMode(IInterfaceLogger logger, MeasurementModes mode) =>
        httpClient.PutAsync($"{new Uri(_baseUri, "MeasurementMode")}?mode={JsonConvert.SerializeObject(mode)}", null);
}
