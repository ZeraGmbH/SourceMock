using ErrorCalculatorApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class ErrorCalculatorMock : IErrorCalculator
{
    /// <summary>
    /// 
    /// </summary>
    public bool Available => true;

    private bool _continious = false;
    private ErrorMeasurementStatus _status = new();
    private ISource _source;
    private DateTime _startTime;
    private double _meterConstant;
    private long _totalImpulses;
    private Loadpoint? _loadpoint;

    /// <summary>
    /// Need SimulatedSource to mock the energy
    /// </summary>
    /// <param name="source"></param>
    public ErrorCalculatorMock(ISource source)
    {
        _source = source;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task AbortErrorMeasurement()
    {
        return Task<bool>.FromResult(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        double power = 0;
        foreach (var phase in _loadpoint!.Phases)
        {
            power += phase.Voltage.Rms * phase.Current.Rms * Math.Cos((phase.Voltage.Angle - phase.Current.Angle) * Math.PI / 180d);
        }
        var timeInSeconds = (DateTime.Now - _startTime).TotalSeconds;

        double energy = power * timeInSeconds / 3600 / 1000;
        _status.Energy = energy;
        var measuredImpulses = _meterConstant * energy;

        _status.Progress = measuredImpulses / _totalImpulses * 100;

        if (_status.Progress >= 100)
        {
            _status.ErrorValue = _status.Progress / 100;
            _status.State = ErrorMeasurementStates.Finished;
        }

        return Task.FromResult<ErrorMeasurementStatus>(_status);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meterConstant"></param>
    /// <param name="impulses"></param>
    /// <returns></returns>
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        _meterConstant = meterConstant;
        _totalImpulses = impulses;
        _loadpoint = _source.GetCurrentLoadpoint();

        return Task<Tuple<double, long>>.FromResult((_meterConstant, _totalImpulses));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="continuous"></param>
    /// <returns></returns>
    public Task StartErrorMeasurement(bool continuous)
    {
        _continious = continuous;
        _startTime = DateTime.Now;
        _status.State = ErrorMeasurementStates.Running;

        return Task<bool>.FromResult(_continious);
    }

}
