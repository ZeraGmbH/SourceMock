using RefMeterApi.Models;
using SharedLibrary.Models.Logging;
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
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => true;

    /// <summary>
    /// MeasurementMode
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes?> GetActualMeasurementMode(IInterfaceLogger logger) =>
        Task.FromResult((MeasurementModes?)_measurementMode);

    /// <inheritdoc/>
    public Task<double> GetMeterConstant(IInterfaceLogger logger) => Task.FromResult(1000000d);

    /// <summary>
    /// Returns all entrys in enum MeasurementModes
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes[]> GetMeasurementModes(IInterfaceLogger logger) =>
        Task.FromResult((MeasurementModes[])Enum.GetValues(typeof(MeasurementModes)));

    /// <summary>
    /// Measurement mode is not relevant for mock logic but frontent requeires an implementation
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mode">Real RefMeter requieres a mode</param>
    /// <returns>Must return something - no async task requeired without device</returns>
    public Task SetActualMeasurementMode(IInterfaceLogger logger, MeasurementModes mode)
    {
        _measurementMode = mode;

        return Task.CompletedTask;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>ActualValues that fluctuate around the set loadpoint</returns>
    public abstract Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveVoltagePhase);


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
