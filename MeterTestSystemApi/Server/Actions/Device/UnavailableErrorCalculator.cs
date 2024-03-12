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

    public Task<ErrorMeasurementStatus> GetErrorStatus() => throw new ErrorCalculatorNotReadyException();

    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion() => throw new NotImplementedException();

    public Task SetErrorMeasurementParameters(double meterConstant, long impulses) => throw new ErrorCalculatorNotReadyException();

    public Task StartErrorMeasurement(bool continuous) => throw new ErrorCalculatorNotReadyException();
}
