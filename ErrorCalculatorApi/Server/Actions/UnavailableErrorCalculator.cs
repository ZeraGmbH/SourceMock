using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Exceptions;
using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions;

/// <summary>
/// Implementation of an error calculator not yet configured.
/// </summary>
public class UnavailableErrorCalculator : IErrorCalculator
{
    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => false;

    /// <inheritdoc/>
    public Task AbortErrorMeasurement(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task StartErrorMeasurement(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task<long?> GetNumberOfDeviceUnderTestImpulses(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task AbortAllJobs(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task ActivateSource(IInterfaceLogger logger, bool on) => Task.CompletedTask;
}