using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Using MAD 1.04 XML communication with an error calculator.
/// </summary>
public class Mad1ErrorCalculator : IErrorCalculatorInternal
{
    /// <inheritdoc/>
    public bool Available => false;

    /// <inheritdoc/>
    public Task AbortErrorMeasurement()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Initialize(ErrorCalculatorConfiguration configuration, IServiceProvider services)
    {
        throw new NotImplementedException();
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