using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Exceptions;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions;

/// <summary>
/// Implementation of an error calculator not yet configured.
/// </summary>
public class UnavailableErrorCalculator : IErrorCalculator
{
    /// <inheritdoc/>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(false);

    /// <inheritdoc/>
    public Task AbortErrorMeasurementAsync(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatusAsync(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task SetErrorMeasurementParametersAsync(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task StartErrorMeasurementAsync(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnectionsAsync(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task<Impulses?> GetNumberOfDeviceUnderTestImpulsesAsync(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task AbortAllJobsAsync(IInterfaceLogger logger) => throw new ErrorCalculatorNotReadyException();

    /// <inheritdoc/>
    public Task ActivateSourceAsync(IInterfaceLogger logger, bool on) => Task.CompletedTask;
}