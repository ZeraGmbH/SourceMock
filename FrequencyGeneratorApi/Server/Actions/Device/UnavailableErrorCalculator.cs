using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;

namespace MeteringSystemApi;

internal class UnavailableErrorCalculator : IErrorCalculator
{
    public bool Available => false;

    public Task AbortErrorMeasurement()
    {
        throw new NotImplementedException();
    }

    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        throw new NotImplementedException();
    }

    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        throw new NotImplementedException();
    }

    public Task StartErrorMeasurement(bool continuous)
    {
        throw new NotImplementedException();
    }
}
