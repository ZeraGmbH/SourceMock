using System.Xml;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Using MAD 1.04 XML communication with an error calculator.
/// </summary>
public partial class Mad1ErrorCalculator : IErrorCalculatorInternal
{
    /// <inheritdoc/>
    public bool Available => false;

    private IMadConnection _connection = null!;

    /// <inheritdoc/>
    public Task AbortErrorMeasurement()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
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
    public async Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion()
    {
        /* Execute the request. */
        await _connection.Execute(GetVersionRequest());

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
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous)
    {
        throw new NotImplementedException();
    }
}