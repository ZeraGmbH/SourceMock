using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Exceptions;
using ErrorCalculatorApi.Models;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implementation of an error calculator not yet configured.
/// </summary>
internal class UnavailableErrorCalculator : IErrorCalculator
{
    public bool Available => false;

    public Task AbortErrorMeasurement() => throw new ErrorCalculatorNotReadyException();

    public void Dispose()
    {
    }

    public Task<ErrorMeasurementStatus> GetErrorStatus() => throw new ErrorCalculatorNotReadyException();

    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion() => throw new ErrorCalculatorNotReadyException();

    public Task SetErrorMeasurementParameters(double dutMeterConstant, long impulses, double refMeterMeterConstant) => throw new ErrorCalculatorNotReadyException();

    public Task StartErrorMeasurement(bool continuous, ErrorCalculatorConnections? connection) => throw new ErrorCalculatorNotReadyException();

    public Task<ErrorCalculatorConnections[]> GetSupportedConnections() => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task AbortAllJobs() => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task ActivateSource(bool on) => Task.CompletedTask;
}
