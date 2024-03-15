using Microsoft.Extensions.Options;
using RefMeterApi.Models;
using SourceApi.Model;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public abstract class RefMeterMock : IMockRefMeter
{
    /// <summary>
    /// 
    /// </summary>
    protected MeasurementModes _measurementMode = MeasurementModes.FourWireActivePower;

    /// <summary>
    /// 
    /// </summary>
    public bool Available => true;

    /// <summary>
    /// MeasurementMode
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes?> GetActualMeasurementMode() =>
        Task.FromResult((MeasurementModes?)_measurementMode);

    /// <inheritdoc/>
    public Task<double> GetMeterConstant() => Task.FromResult(1000000d);

    /// <summary>
    /// Returns all entrys in enum MeasurementModes
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes[]> GetMeasurementModes() =>
        Task.FromResult((MeasurementModes[])Enum.GetValues(typeof(MeasurementModes)));

    /// <summary>
    /// Measurement mode is not relevant for mock logic but frontent requeires an implementation
    /// </summary>
    /// <param name="mode">Real RefMeter requieres a mode</param>
    /// <returns>Must return something - no async task requeired without device</returns>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        _measurementMode = mode;

        return Task.CompletedTask;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ActualValues that fluctuate around the set loadpoint</returns>
    public abstract Task<MeasuredLoadpoint> GetActualValues(int firstActiveVoltagePhase);


    /// <summary>
    /// Calculates an expected Measure Output from a given loadpoint.
    /// </summary>
    /// <param name="lp">The loadpoint.</param>
    /// <returns>The according measure output.</returns>
    public abstract MeasuredLoadpoint CalcMeasureOutput(TargetLoadpoint lp);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="deviation"></param>
    /// <returns></returns>
    protected static double GetRandomNumberWithAbsoluteDeviation(double value, double deviation)
    {
        var maximum = value + deviation;
        var minimum = value - deviation;
        var random = Random.Shared;

        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="deviation"></param>
    /// <returns></returns>
    protected static double GetRandomNumberWithPercentageDeviation(double value, double deviation)
    {
        var maximum = value + value * deviation / 100;
        var minimum = value - value * deviation / 100;
        var random = Random.Shared;

        return random.NextDouble() * (maximum - minimum) + minimum;
    }
}
