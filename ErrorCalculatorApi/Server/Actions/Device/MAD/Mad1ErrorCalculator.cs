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

    /// <inheritdoc/>
    public Task AbortErrorMeasurement()
    {
        throw new NotImplementedException();
    }

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
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Initialize(ErrorCalculatorConfiguration configuration, IServiceProvider services)
    {
        /* Create connection implementation. */
        _connection = services.GetRequiredKeyedService<IMadConnection>(ErrorCalculatorConnectionTypes.TCP);

        return _connection.Initialize(configuration);
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double dutMeterConstant, long impulse, double refMeterMeterConstant)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorConnections[]> GetSupportedConnections() => Task.FromResult(Enum.GetValues<ErrorCalculatorConnections>());

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous, ErrorCalculatorConnections? connection)
    {
        throw new NotImplementedException();
    }
}