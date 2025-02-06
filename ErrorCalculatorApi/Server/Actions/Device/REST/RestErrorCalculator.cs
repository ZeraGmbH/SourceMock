using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;
using ZeraDevices.ErrorCalculator.STM;

namespace ErrorCalculatorApi.Actions.Device.REST;

/// <summary>
/// Error calculator interface for external implementations
/// based on a REST interface.
/// </summary>
public class RestErrorCalculator(ILoggingHttpClient errorCalculator, ILogger<RestErrorCalculator> logger) : IErrorCalculatorInternalLegacy
{
    private bool _initialized = false;

    private Uri _uri = null!;

    private static string ToUrl<T>(T value) => JsonSerializer.Serialize(value, LibUtils.JsonSettings);

    /// <inheritdoc/>
    public Task AbortAllJobsAsync(IInterfaceLogger interfaceLogger)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, "AbortAll"));

    /// <inheritdoc/>
    public Task AbortErrorMeasurementAsync(IInterfaceLogger interfaceLogger)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, "Abort"));

    /// <inheritdoc/>
    public Task ActivateSourceAsync(IInterfaceLogger interfaceLogger, bool on)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, on ? "ActivateSource" : "DeactivateSource"));

    /// <inheritdoc/>
    public async Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger)
    {
        /* Not yet initialized. */
        if (!_initialized) return false;

        try
        {
            return await errorCalculator.GetAsync<bool>(interfaceLogger, new Uri(_uri, "Available"));
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
    public Task<ErrorMeasurementStatus> GetErrorStatusAsync(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<ErrorMeasurementStatus>(interfaceLogger, _uri);

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<ErrorCalculatorFirmwareVersion>(interfaceLogger, new Uri(_uri, "Version"));

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnectionsAsync(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<ErrorCalculatorMeterConnections[]>(interfaceLogger, new Uri(_uri, "GetSupportedMeterConnections"));

    /// <inheritdoc/>
    public Task<Impulses?> GetNumberOfDeviceUnderTestImpulsesAsync(IInterfaceLogger interfaceLogger)
        => errorCalculator.GetAsync<Impulses?>(interfaceLogger, new Uri(_uri, "DutImpulses"));

    /// <inheritdoc/>
    public Task SetErrorMeasurementParametersAsync(IInterfaceLogger interfaceLogger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant)
        => errorCalculator.PutAsync(interfaceLogger, new Uri(_uri, $"?dutMeterConstant={ToUrl(dutMeterConstant)}&impulses={ToUrl(impulses)}&refMeterMeterConstant={ToUrl(refMeterMeterConstant)}"));

    /// <inheritdoc/>
    public Task StartErrorMeasurementAsync(IInterfaceLogger interfaceLogger, bool continuous, ErrorCalculatorMeterConnections? connection)
        => errorCalculator.PostAsync(interfaceLogger, new Uri(_uri, $"{(continuous ? "StartContinuous" : "StartSingle")}{(connection.HasValue ? $"?connection={ToUrl(connection)}" : string.Empty)}"));

    /// <inheritdoc/>
    public Task InitializeAsync(int position, ErrorCalculatorConfiguration configuration, IServiceProvider services)
    {
        /* Can be only done once. */
        if (_initialized) throw new InvalidOperationException("Already initialized");

        /* Reset - just in case!. */
        _uri = null!;

        /* Validate. */
        if (string.IsNullOrEmpty(configuration?.Endpoint) || configuration.Protocol != ErrorCalculatorProtocols.HTTP)
            throw new InvalidOperationException("no error calculator connection configured");

        /* Configure connection for logging. */
        errorCalculator.LogConnection = new()
        {
            Endpoint = configuration.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Http,
            WebSamType = InterfaceLogSourceTypes.ErrorCalculator,
        };

        _uri = new Uri(configuration.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_uri.UserInfo))
            errorCalculator.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_uri.UserInfo)));

        /* Did it. */
        _initialized = true;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Destroy() { }
}