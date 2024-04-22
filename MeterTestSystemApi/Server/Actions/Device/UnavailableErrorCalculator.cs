using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Exceptions;
using ErrorCalculatorApi.Models;
using SharedLibrary.Models.Logging;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Implementation of an error calculator not yet configured.
/// </summary>
internal class UnavailableErrorCalculator : IErrorCalculator
{
    public bool Available => false;

    public Task AbortErrorMeasurement(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    public void Dispose()
    {
    }

    public Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    public Task SetErrorMeasurementParameters(IInterfaceLogger logger, double dutMeterConstant, long impulses, double refMeterMeterConstant) => throw new ErrorCalculatorNotReadyException();

    public Task StartErrorMeasurement(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection) => throw new ErrorCalculatorNotReadyException();

    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections() => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task AbortAllJobs(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task ActivateSource(IInterfaceLogger logger, bool on) => Task.CompletedTask;
}
