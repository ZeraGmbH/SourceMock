using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
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
    public bool Available => true;

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
    public Task AbortErrorMeasurement()
    {
        _status.State = ErrorMeasurementStates.Finished;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        /* Time elapses in the current measurement - the mock does not use it's own thred for timing so calling this methode periodically is vital. */
        var hoursElapsed = (DateTime.UtcNow - _startTime).TotalHours;

        /* Total energy consumed in kWh and store to status. */
        var energy = hoursElapsed * _totalPower;

        _status.Energy = 1000d * energy;

        /* Get the number of pulses and from this the progress. */
        var measuredImpulses = (long)(_meterConstant * energy);

        if (measuredImpulses > _totalImpulses)
        {
            /* In mock never report more than requested - even if time has run out. */
            measuredImpulses = _totalImpulses;

            _status.Energy = 1000d * _totalImpulses / _meterConstant;
        }

        _status.Progress = 100d * measuredImpulses / _totalImpulses;

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
            Energy = _status.Energy,
            ErrorValue = _status.ErrorValue,
            Progress = _status.Progress,
            State = _status.State,
        });
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        _meterConstant = meterConstant;
        _totalImpulses = impulses;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous)
    {
        /* Get the total power of all active phases of the current loadpoint (in W) */
        var totalPower = 0d;

        var loadpoint = _di.GetRequiredService<ISource>().GetCurrentLoadpoint()!;

        foreach (var phase in loadpoint.Phases)
            if (phase.Voltage.On && phase.Current.On)
                totalPower += phase.Voltage.AcComponent.Rms * phase.Current.AcComponent.Rms *
                Math.Cos((phase.Voltage.AcComponent.Angle - phase.Current.AcComponent.Angle) * Math.PI / 180d);

        /* Use total power in kW to ease calculations with meter constant. */
        _totalPower = totalPower / 1000d;
        _continuous = continuous;

        _startTime = DateTime.UtcNow;

        _status = new() { State = ErrorMeasurementStates.Running, Progress = 0, ErrorValue = null };

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion() =>
        Task.FromResult(new ErrorCalculatorFirmwareVersion()
        {
            ModelName = "CalculatorMock",
            Version = "1.0"
        });

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
