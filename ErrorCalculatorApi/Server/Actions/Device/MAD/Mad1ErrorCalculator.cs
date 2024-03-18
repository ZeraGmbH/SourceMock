using System.Xml;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorCalculatorApi.Actions.Device.MAD;

/// <summary>
/// Using MAD 1.04 XML communication with an error calculator.
/// </summary>
public partial class Mad1ErrorCalculator : IErrorCalculatorInternal
{
    /// <inheritdoc/>
    public bool Available => _connection?.Available == true;

    private IMadConnection _connection = null!;

    private long? _dutImpules;

    private long? _dutImpulesNext;

    private long? _refMeterImpulsesNext;

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

        return _connection.Initialize(configuration);
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double dutMeterConstant, long impulses, double refMeterMeterConstant)
    {
        /* Remember */
        _refMeterImpulsesNext = (long)Math.Round(impulses * refMeterMeterConstant / dutMeterConstant);
        _dutImpulesNext = impulses;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections() => Task.FromResult(_supportedMeterConnections.Keys.ToArray());

    /// <inheritdoc/>
    public async Task StartErrorMeasurement(bool continuous, ErrorCalculatorMeterConnections? connection)
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
        await ConfigureErrorMeasurement(connection);

        /* Start the measurement and remember context. */
        _jobId = await StartErrorMeasurement(continuous, connection, (long)dut, (long)meter);

        _dutImpules = dut;
    }
}