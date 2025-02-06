using System.Xml;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Provider;

namespace ZeraDevices.ErrorCalculator.STM;

/// <summary>
/// Configuration interface for an error calculator.
/// </summary>
public interface IErrorCalculatorInternal : IErrorCalculator
{
    /// <summary>
    /// Configure a brand new error calculator.
    /// </summary>
    /// <param name="position">Position of the test system.</param>
    /// <param name="endpoint">Endpoint to connect to.</param>
    /// <param name="services">Dependency injection.</param>
    Task InitializeAsync(int position, string endpoint, IServiceProvider services);

    /// <summary>
    /// Release all resources.
    /// </summary>
    void Destroy();
}

/// <summary>
/// Using MAD 1.04 XML communication with an error calculator.
/// </summary>
public partial class Mad1ErrorCalculator : IErrorCalculatorInternal
{
    /// <inheritdoc/>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(_connection?.Available == true);

    private IMadConnection _connection = null!;

    private Impulses? _dutImpules;

    private Impulses? _dutImpulesNext;

    private Impulses? _refMeterImpulsesNext;

    private string? _jobId;

    private int _position;

    /// <inheritdoc/>
    public void Destroy()
    {
        using (_connection)
            _connection = null!;
    }

    private static XmlDocument LoadXmlFromString(string xml)
    {
        var doc = new XmlDocument();

        doc.LoadXml(xml);

        return doc;
    }

    /// <inheritdoc/>
    public Task InitializeAsync(int position, string endpoint, IServiceProvider services)
    {
        _position = position;

        /* Create connection implementation. */
        _connection = services.GetRequiredKeyedService<IMadConnection>(ErrorCalculatorConnectionTypes.TCP);

        return _connection.InitializeAsync($"{position}", endpoint);
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParametersAsync(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant)
    {
        /* Remember */
        _refMeterImpulsesNext = (impulses / dutMeterConstant * refMeterMeterConstant).Floor();
        _dutImpulesNext = impulses.Floor();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnectionsAsync(IInterfaceLogger logger) => Task.FromResult(_supportedMeterConnections.Keys.ToArray());

    /// <inheritdoc/>
    public async Task StartErrorMeasurementAsync(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection)
    {
        /* Already running. */
        if (!string.IsNullOrEmpty(_jobId)) throw new InvalidOperationException("error measurement still active - abort first");

        /* Validate. */
        var dut = _dutImpulesNext;
        var meter = _refMeterImpulsesNext;

        _dutImpulesNext = null;
        _refMeterImpulsesNext = null;

        if (dut == null || meter == null) throw new InvalidOperationException("error measurement not configured");

        /* Configure the measurement. */
        await ConfigureErrorMeasurementAsync(logger, connection);

        /* Start the measurement and remember context. */
        _jobId = await StartErrorMeasurementAsync(logger, continuous, connection, dut.Value, meter.Value);

        _dutImpules = dut;
    }

    /// <inheritdoc/>
    public Task<Impulses?> GetNumberOfDeviceUnderTestImpulsesAsync(IInterfaceLogger logger) => Task.FromResult<Impulses?>(null);

    /* [TODO] add support for 0x HEX response. */
    private static long ParseLong(string number) => long.Parse(number);
}