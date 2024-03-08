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

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Meter test system based on REST connection to source and reference meter.
/// </summary>
/// <param name="httpClient">Connection to a remote meter test system.</param>
public class RestMeterTestSystem(HttpClient httpClient) : IMeterTestSystem
{
    private Uri _baseUri = null!;

    /// <inheritdoc/>
    public AmplifiersAndReferenceMeter AmplifiersAndReferenceMeter => throw new NotImplementedException();

    /// <inheritdoc/>
    public ISource Source { get; private set; } = new UnavailableSource();

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; private set; } = new UnavailableReferenceMeter();

    private readonly List<IErrorCalculator> _errorCalculators = [new UnavailableErrorCalculator()];

    /// <inheritdoc/>
    public IErrorCalculator[] ErrorCalculators => _errorCalculators.ToArray();

    /// <inheritdoc/>
#pragma warning disable CS0414
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilities() =>
        Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditions() =>
        httpClient.GetAsync(new Uri(_baseUri, "ErrorConditions")).GetJsonResponse<ErrorConditions>();

    /// <inheritdoc/>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion() =>
        httpClient.GetAsync(new Uri(_baseUri, "FirmwareVersion")).GetJsonResponse<MeterTestSystemFirmwareVersion>();

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeter settings)
    {
        /* The fallback do not support amplifier configurations. */
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Configure all sub components.
    /// </summary>
    /// <param name="config">Configuration to use.</param>
    /// <param name="di">Active dependency injection runtime to create helper.</param>
    public void Configure(InterfaceConfiguration config, IServiceProvider di)
    {
        /* Validate. */
        if (string.IsNullOrEmpty(config.MeterTestSystem?.EndPoint)) throw new InvalidOperationException("no meter test system connection configured");

        _baseUri = new Uri(config.MeterTestSystem.EndPoint.TrimEnd('/') + "/");

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

        /* Use. */
        RefMeter = refMeter;
        Source = source;
    }
}
