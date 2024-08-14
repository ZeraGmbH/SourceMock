using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions;

/// <summary>
/// Error calculator interface for external implementations
/// based on a REST interface.
/// </summary>
public class RestErrorCalculator(ILoggingHttpClient errorCalculator, ILogger<RestErrorCalculator> logger) : IRestErrorCalculator
{
    private bool _initialized = false;

    private Uri _uri = null!;

    private static string ToUrl<T>(T value) => JsonSerializer.Serialize(value, LibUtils.JsonSettings);

    /// <inheritdoc/>
    public Task AbortAllJobs(IInterfaceLogger interfaceLogger)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, "AbortAll"));

    /// <inheritdoc/>
    public Task AbortErrorMeasurement(IInterfaceLogger interfaceLogger)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, "Abort"));

    /// <inheritdoc/>
    public Task ActivateSource(IInterfaceLogger interfaceLogger, bool on)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, on ? "ActivateSource" : "DeactivateSource"));

    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger)
    {
        /* Not yet initialized. */
        if (!_initialized) return false;

        try
        {
            var available = errorCalculator.GetAsync<bool>(interfaceLogger, new Uri(_uri, "Available"));

            available.Wait();

            return available.Result;
        }
        catch (Exception e)
        {
            /* Just report the error. */
            logger.LogError("Unable to connect to remote error calculator API: {Exception}",
                e is AggregateException ae
                ? ae.InnerExceptions[0].Message
                : e.Message);

            return false;
        }
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<ErrorMeasurementStatus>(interfaceLogger, _uri);

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<ErrorCalculatorFirmwareVersion>(interfaceLogger, new Uri(_uri, "Version"));

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<ErrorCalculatorMeterConnections[]>(interfaceLogger, new Uri(_uri, "GetSupportedMeterConnections"));

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(IInterfaceLogger interfaceLogger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant)
        => errorCalculator.PutAsync(interfaceLogger, new Uri(_uri, $"?dutMeterConstant={ToUrl(dutMeterConstant)}&impulses={ToUrl(impulses)}&refMeterMeterConstant?{ToUrl(refMeterMeterConstant)}"));

    /// <inheritdoc/>
    public Task StartErrorMeasurement(IInterfaceLogger interfaceLogger, bool continuous, ErrorCalculatorMeterConnections? connection)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, $"{(continuous ? "StartContinuous" : "StartSingle")}?connection={ToUrl(connection)}"));

    /// <inheritdoc/>
    public void Initialize(RestConfiguration? endpoint)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _uri = null!;

        /* Validate. */
        if (string.IsNullOrEmpty(endpoint?.Endpoint)) throw new InvalidOperationException("no error calculator connection configured");

        /* Configure connection for logging. */
        errorCalculator.LogConnection = new()
        {
            Endpoint = endpoint.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Http,
            WebSamType = InterfaceLogSourceTypes.ErrorCalculator,
        };

        _uri = new Uri(endpoint.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_uri.UserInfo))
            errorCalculator.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_uri.UserInfo)));

        /* Did it. */
        _initialized = true;
    }
}