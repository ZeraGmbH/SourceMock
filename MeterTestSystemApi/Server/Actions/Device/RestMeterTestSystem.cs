using System.Net.Http.Headers;
using System.Text;
using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Actions.Device;
using SourceApi.Actions.RestSource;
using SourceApi.Actions.Source;
using SharedLibrary;
using RefMeterApi.Actions.RestSource;
using Microsoft.Extensions.Logging;
using SharedLibrary.Models.Logging;
using SharedLibrary.Models;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Meter test system based on REST connection to source and reference meter.
/// </summary>
/// <param name="httpClient">Connection to a remote meter test system.</param>
/// <param name="factory">Factory instance to create error calculators.</param>
/// <param name="logger">Loggin helper.</param>
public class RestMeterTestSystem(ILoggingHttpClient httpClient, IErrorCalculatorFactory factory, ILogger<RestMeterTestSystem> logger) : IMeterTestSystem
{
    private Uri _baseUri = null!;

    /// <inheritdoc/>
    public AmplifiersAndReferenceMeter GetAmplifiersAndReferenceMeter(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public ISource Source { get; private set; } = new UnavailableSource();

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; private set; } = new UnavailableReferenceMeter();

    private readonly List<IErrorCalculator> _errorCalculators = [new UnavailableErrorCalculator()];

    /// <inheritdoc/>
    public IErrorCalculator[] ErrorCalculators => [.. _errorCalculators];

    /// <inheritdoc/>
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger) =>
        Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public async Task<ErrorConditions> GetErrorConditions(IInterfaceLogger logger)
    {
        var errors = await httpClient.GetAsync<ErrorConditions>(logger, new Uri(_baseUri, "ErrorConditions"));

        ErrorConditionsChanged?.Invoke(errors);

        return errors;
    }

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) =>
        httpClient.GetAsync<MeterTestSystemFirmwareVersion>(logger, new Uri(_baseUri, "FirmwareVersion"));

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings)
    {
        /* The fallback do not support amplifier configurations. */
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Configure all sub components.
    /// </summary>
    /// <param name="config">Configuration to use.</param>
    /// <param name="di">Active dependency injection runtime to create helper.</param>
    public async void Configure(InterfaceConfiguration config, IServiceProvider di)
    {
        /* Validate. */
        if (string.IsNullOrEmpty(config.MeterTestSystem?.Endpoint))
        {
            /* Repot but start to allow correction of configuration. */
            logger.LogCritical("no meter test system connection configured");

            return;
        }

        /* Configure connection for logging. */
        httpClient.LogConnection = new()
        {
            Endpoint = config.MeterTestSystem.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Http,
            WebSamType = InterfaceLogSourceTypes.MeterTestSystem,
        };

        _baseUri = new Uri(config.MeterTestSystem.Endpoint.TrimEnd('/') + "/");

        /* May have authorisation. */
        if (!string.IsNullOrEmpty(_baseUri.UserInfo))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_baseUri.UserInfo)));

        /* Create. */
        var source = di.GetRequiredService<IRestSource>();
        var refMeter = di.GetRequiredService<IRestRefMeter>();

        /* Configure. */
        source.Initialize(config.Source, config.Dosage);
        refMeter.Initialize(config.ReferenceMeter);

        /* Error calculators. */
        var errorCalculators = new List<IErrorCalculatorInternal>();

        try
        {
            /* Create calculators based on configuration. */
            for (var i = 0; i < config.ErrorCalculators.Count; i++)
                errorCalculators.Add(await factory.Create(i, config.ErrorCalculators[i]));
        }
        catch (Exception e)
        {
            /* Release anything we have configured so far. */
            errorCalculators.ForEach(ec => ec.Destroy());

            /* Repot but start to allow correction of configuration. */
            logger.LogCritical("unable to attach error calculators: {Exception}", e.Message);

            return;
        }

        /* Use. */
        RefMeter = refMeter;
        Source = source;

        _errorCalculators.Clear();
        _errorCalculators.AddRange(errorCalculators);
    }
}
