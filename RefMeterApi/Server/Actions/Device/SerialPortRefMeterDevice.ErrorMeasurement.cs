using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortRefMeterDevice
{

    /// <inheritdoc/>
    public Task AbortErrorMeasurement()
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        return Task.FromResult(new ErrorMeasurementStatus
        {
            Energy = 0,
            ErrorValue = 0,
            Progress = 0,
            State = ErrorMeasurementStates.NotActive
        });
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous)
    {
        return Task.CompletedTask;
    }
}
