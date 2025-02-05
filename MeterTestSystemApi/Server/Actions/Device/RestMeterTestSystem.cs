using ErrorCalculatorApi.Actions;
using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions;
using RefMeterApi.Actions.RestSource;
using SourceApi.Actions;
using SourceApi.Actions.RestSource;
using SourceApi.Actions.Source;
using ZERA.WebSam.Shared.Provider;
using System.Net.Http.Headers;
using System.Text;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.MeterTestSystem;

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
    public Task<AmplifiersAndReferenceMeter> GetAmplifiersAndReferenceMeterAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool HasSource { get; private set; } = false;

    /// <inheritdoc/>
    public bool HasDosage { get; private set; } = false;

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
    public Task<MeterTestSystemCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) =>
        Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public async Task<ErrorConditions> GetErrorConditionsAsync(IInterfaceLogger logger)
    {
        var errors = await httpClient.GetAsync<ErrorConditions>(logger, new Uri(_baseUri, "ErrorConditions"));

        ErrorConditionsChanged?.Invoke(errors);

        return errors;
    }

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger) =>
        httpClient.GetAsync<MeterTestSystemFirmwareVersion>(logger, new Uri(_baseUri, "FirmwareVersion"));

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeterAsync(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings)
    {
        /* The fallback do not support amplifier configurations. */
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Configure all sub components.
    /// </summary>
    /// <param name="config">Configuration to use.</param>
    /// <param name="di">Active dependency injection runtime to create helper.</param>
    /// <param name="interfaceLogger"></param>
    public async Task ConfigureAsync(InterfaceConfiguration config, IServiceProvider di, IInterfaceLogger interfaceLogger)
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

        /* Dosage is optional. */
        IDosage? dosage = null;

        if (!string.IsNullOrEmpty(config.Dosage?.Endpoint))
        {
            /* Full REST dosage API implementation. */
            var restDosage = di.GetRequiredService<IRestDosage>();

            restDosage.Initialize(config.Dosage);

            /* Use this. */
            dosage = restDosage;

            HasDosage = true;
        }

        /* No source - currently this disables dosage as well which must be improved. */
        ISource source;

        if (string.IsNullOrEmpty(config.Source?.Endpoint))
            source = new UnavailableSource(dosage);
        else
        {
            /* Full REST source API implementation. */
            var restSource = di.GetRequiredService<IRestSource>();

            restSource.Initialize(config.Source, dosage);

            /* Use this. */
            source = restSource;

            HasSource = true;
        }

        /* May want to disable source usage in dosage if mocking the devices. */
        if (HasDosage && !HasSource)
            if (dosage is IDosageMock dosageMock)
                await dosageMock.NoSourceAsync(interfaceLogger);

        /* Reference meter. */
        var refMeter = di.GetRequiredService<IRestRefMeter>();

        refMeter.Initialize(config.ReferenceMeter);

        /* Error calculators. */
        var errorCalculators = new List<IErrorCalculatorInternal>();

        try
        {
            /* Create calculators based on configuration. */
            for (var i = 0; i < config.ErrorCalculators.Count; i++)
                errorCalculators.Add(await factory.CreateAsync(i, config.ErrorCalculators[i]));
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
