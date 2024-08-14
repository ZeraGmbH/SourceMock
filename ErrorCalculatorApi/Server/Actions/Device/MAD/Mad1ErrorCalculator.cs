using System.Xml;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device.MAD;

/// <summary>
/// Using MAD 1.04 XML communication with an error calculator.
/// </summary>
public partial class Mad1ErrorCalculator : IErrorCalculatorInternal
{
    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => _connection?.Available == true;

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
    public Task Initialize(int position, ErrorCalculatorConfiguration configuration, IServiceProvider services)
    {
        _position = position;

        /* Create connection implementation. */
        _connection = services.GetRequiredKeyedService<IMadConnection>(ErrorCalculatorConnectionTypes.TCP);

        return _connection.Initialize($"{position}", configuration);
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant)
    {
        /* Remember */
        _refMeterImpulsesNext = (impulses / dutMeterConstant * refMeterMeterConstant).Floor();
        _dutImpulesNext = impulses.Floor();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections(IInterfaceLogger logger) => Task.FromResult(_supportedMeterConnections.Keys.ToArray());

    /// <inheritdoc/>
    public async Task StartErrorMeasurement(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection)
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
        await ConfigureErrorMeasurement(logger, connection);

        /* Start the measurement and remember context. */
        _jobId = await StartErrorMeasurement(logger, continuous, connection, dut.Value, meter.Value);

        _dutImpules = dut;
    }

    /// <inheritdoc/>
    public Task<long?> GetNumberOfDeviceUnderTestImpulses(IInterfaceLogger logger) => Task.FromResult<long?>(null);

    /* [TODO] add support for 0x HEX response. */
    private static long ParseLong(string number) => long.Parse(number);
}