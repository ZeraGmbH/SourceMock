using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Tag interface
/// </summary>
public interface IErrorCalculatorMock : IErrorCalculator { }
/// <summary>
/// 
/// </summary>
public class ErrorCalculatorMock : IErrorCalculatorMock
{
    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => true;

    private bool _continuous = false;

    private ErrorMeasurementStatus _status = new();

    private IServiceProvider _di;

    private DateTime _startTime;

    private double _meterConstant;

    private long _totalImpulses;

    /// <summary>
    /// Total power of the loadpoint in kW.
    /// </summary>
    private double _totalPower;

    /// <summary>
    /// Need SimulatedSource to mock the energy
    /// </summary>
    /// <param name="di">Dependency injection to create ISource on demand -
    /// to avoid cyclic dependencies during setup.</param>
    public ErrorCalculatorMock(IServiceProvider di)
    {
        _di = di;
    }

    /// <inheritdoc/>
    public Task AbortErrorMeasurement(IInterfaceLogger logger)
    {
        _status.State = ErrorMeasurementStates.Finished;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task AbortAllJobs(IInterfaceLogger logger) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger logger)
    {
        /* Time elapses in the current measurement - the mock does not use it's own thred for timing so calling this methode periodically is vital. */
        var hoursElapsed = (DateTime.UtcNow - _startTime).TotalHours;

        /* Total energy consumed in kWh and store to status. */
        var energy = hoursElapsed * _totalPower;


        /* Get the number of pulses and from this the progress. */
        var measuredImpulses = (long)(_meterConstant * energy);

        if (measuredImpulses > _totalImpulses)
        {
            /* In mock never report more than requested - even if time has run out. */
            measuredImpulses = _totalImpulses;
        }

        _status.Progress = 100d * measuredImpulses / _totalImpulses;
        _status.ReferenceCountsOrEnergy = 1000d * measuredImpulses / _meterConstant;
        _status.MeterCountsOrEnergy = _status.ReferenceCountsOrEnergy;

        /* Check for end of measurement. */
        if (_status.Progress >= 100)
        {
            /* Keep this in continuous mode. */
            _status.ErrorValue = Random.Shared.Next(9500, 10700) / 100d - 100d;
            _status.Progress = 0d;

            /* Restart counting or end measurement. */
            if (_continuous)
                _startTime = DateTime.UtcNow;
            else
                _status.State = ErrorMeasurementStates.Finished;
        }

        /* Report copy - never give access to internal structures. */
        return Task.FromResult(new ErrorMeasurementStatus
        {
            CountsAreEnergy = true,
            ErrorValue = _status.ErrorValue,
            MeterCountsOrEnergy = _status.MeterCountsOrEnergy,
            Progress = _status.Progress,
            ReferenceCountsOrEnergy = _status.ReferenceCountsOrEnergy,
            State = _status.State,
        });
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(IInterfaceLogger logger, double dutMeterConstant, long impulses, double refMeterMeterConstant)
    {
        _meterConstant = dutMeterConstant;
        _totalImpulses = impulses;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections() => Task.FromResult<ErrorCalculatorMeterConnections[]>([]);

    /// <inheritdoc/>
    public Task StartErrorMeasurement(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection)
    {
        /* Get the total power of all active phases of the current loadpoint (in W) */
        var totalPower = 0d;

        var loadpoint = _di.GetRequiredService<ISource>().GetCurrentLoadpoint(logger)!;

        foreach (var phase in loadpoint.Phases)
            if (phase.Voltage.On && phase.Current.On)
            {
                totalPower = CurrentCalculation.CalculateAcPower(totalPower, phase);
                totalPower = CurrentCalculation.CalculateDcPower(totalPower, phase);
            }

        /* Use total power in kW to ease calculations with meter constant. */
        _totalPower = totalPower / 1000d;
        _continuous = continuous;

        _startTime = DateTime.UtcNow;

        _status = new() { State = ErrorMeasurementStates.Running, Progress = 0, ErrorValue = null };

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) =>
        Task.FromResult(new ErrorCalculatorFirmwareVersion()
        {
            ModelName = "CalculatorMock",
            Version = "1.0"
        });

    /// <inheritdoc/>
    public Task ActivateSource(IInterfaceLogger logger, bool on) => Task.CompletedTask;

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
