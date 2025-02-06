using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.Provider;

namespace MockDevices.ErrorCalculator;

/// <summary>
/// Tag interface
/// </summary>
public interface IErrorCalculatorMock : IErrorCalculator { }

/// <summary>
/// Need SimulatedSource to mock the energy
/// </summary>
/// <param name="di">Dependency injection to create ISource on demand -
/// to avoid cyclic dependencies during setup.</param>
public class ErrorCalculatorMock(IServiceProvider di) : IErrorCalculatorMock
{
    private readonly object _pulseLock = new();

    private DateTime _pulseTime = DateTime.UtcNow;

    private long _dutPulses = Environment.TickCount64;

    /// <inheritdoc/>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(true);

    private bool _continuous = false;

    private ErrorMeasurementStatus _status = new();

    private IServiceProvider _di = di;

    private DateTime _startTime;

    private MeterConstant _meterConstant;

    private Impulses _totalImpulses;

    /// <summary>
    /// Total power of the loadpoint in W.
    /// </summary>
    private ActivePower _totalPower;

    /// <inheritdoc/>
    public Task AbortErrorMeasurementAsync(IInterfaceLogger logger)
    {
        _status.State = ErrorMeasurementStates.Finished;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task AbortAllJobsAsync(IInterfaceLogger logger) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task SetErrorMeasurementParametersAsync(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant)
    {
        _meterConstant = dutMeterConstant;
        _totalImpulses = impulses;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnectionsAsync(IInterfaceLogger logger) => Task.FromResult<ErrorCalculatorMeterConnections[]>([]);

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatusAsync(IInterfaceLogger logger)
    {
        /* Time elapses in the current measurement - the mock does not use it's own thred for timing so calling this methode periodically is vital. */
        var elapsed = new Time((DateTime.UtcNow - _startTime).TotalSeconds);

        /* Total energy consumed in kWh and store to status. */
        var energy = _totalPower * elapsed;

        /* Get the number of pulses and from this the progress. */
        var measuredImpulses = (_meterConstant * energy).Abs();

        if (measuredImpulses > _totalImpulses)
        {
            /* In mock never report more than requested - even if time has run out. */
            measuredImpulses = _totalImpulses;
        }

        _status.Progress = measuredImpulses / _totalImpulses * 100d;

        /* Check for end of measurement. */
        if (_status.Progress >= 100)
        {
            /* Keep this in continuous mode. */
            _status.ErrorValue = Random.Shared.Next(9500, 10700) / 100d - 100d;
            _status.Progress = _continuous ? 0d : 100d;

            /* Restart counting or end measurement. */
            if (_continuous)
                _startTime = DateTime.UtcNow;
            else
                _status.State = ErrorMeasurementStates.Finished;
        }

        /* Report copy - never give access to internal structures. */
        return Task.FromResult(new ErrorMeasurementStatus
        {
            ErrorValue = _status.ErrorValue,
            Progress = double.IsNaN(_status.Progress.GetValueOrDefault()) ? null : _status.Progress,
            State = _status.State,
        });
    }

    /// <inheritdoc/>
    public async Task StartErrorMeasurementAsync(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection)
    {
        /* Get the total power of all active phases of the current loadpoint (in W) */
        var totalPower = ActivePower.Zero;

        var source = _di.GetRequiredService<ISource>();
        var loadpoint = await source.GetAvailableAsync(logger) ? await source.GetCurrentLoadpointAsync(logger)! : null;

        if (loadpoint != null)
        {
            /* From loadpoint. */
            foreach (var phase in loadpoint.Phases)
                if (phase.Voltage.On && phase.Current.On)
                {
                    totalPower += CurrentCalculation.CalculateAcPower(phase);
                    totalPower += CurrentCalculation.CalculateDcPower(phase);
                }
        }
        else
        {
            /* From actual values. */
            var values = await _di.GetRequiredService<IRefMeter>().GetActualValuesAsync(logger)!;

            foreach (var phase in values.Phases)
                if (phase.Voltage.AcComponent != null && phase.Current.AcComponent != null)
                {
                    totalPower += CurrentCalculation.CalculateAcPower(phase);
                    totalPower += CurrentCalculation.CalculateDcPower(phase);
                }
        }

        _totalPower = totalPower;
        _continuous = continuous;

        _startTime = DateTime.UtcNow;

        _status = new() { State = ErrorMeasurementStates.Running, Progress = 0, ErrorValue = null };
    }


    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger) =>
        Task.FromResult(new ErrorCalculatorFirmwareVersion()
        {
            ModelName = "CalculatorMock",
            Version = "1.0"
        });

    /// <inheritdoc/>
    public Task ActivateSourceAsync(IInterfaceLogger logger, bool on) => Task.CompletedTask;

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public Task<Impulses?> GetNumberOfDeviceUnderTestImpulsesAsync(IInterfaceLogger logger)
    {
        lock (_pulseLock)
        {
            var now = DateTime.UtcNow;
            var elapsed = (now - _pulseTime).TotalSeconds;

            /* Simulate around 1 pulse per second. */
            if (elapsed > 0)
            {
                _pulseTime = now;
                _dutPulses += (long)Math.Ceiling(elapsed * Random.Shared.Next(90, 110) / 100.0);
            }

            return Task.FromResult<Impulses?>(new Impulses(_dutPulses));
        }
    }
}
