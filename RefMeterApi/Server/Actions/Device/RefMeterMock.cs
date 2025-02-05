using RefMeterApi.Models;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.Source;

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

    private RefMeterStatus _refMeterStatus = new();

    /// <summary>
    /// 
    /// </summary>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(true);

    /// <summary>
    /// MeasurementMode
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes?> GetActualMeasurementModeAsync(IInterfaceLogger logger) =>
        Task.FromResult((MeasurementModes?)_measurementMode);

    /// <inheritdoc/>
    public Task<MeterConstant> GetMeterConstantAsync(IInterfaceLogger logger) => Task.FromResult(new MeterConstant(1000000d));

    /// <summary>
    /// Returns all entrys in enum MeasurementModes
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes[]> GetMeasurementModesAsync(IInterfaceLogger logger) =>
        Task.FromResult((MeasurementModes[])Enum.GetValues(typeof(MeasurementModes)));

    /// <summary>
    /// Measurement mode is not relevant for mock logic but frontent requeires an implementation
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mode">Real RefMeter requieres a mode</param>
    /// <returns>Must return something - no async task requeired without device</returns>
    public Task SetActualMeasurementModeAsync(IInterfaceLogger logger, MeasurementModes mode)
    {
        _measurementMode = mode;
        _refMeterStatus.MeasurementMode = mode;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>ActualValues that fluctuate around the set loadpoint</returns>
    public abstract Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveVoltagePhase);

    /// <inheritdoc/>
    public Task<MeasuredLoadpoint> GetActualValuesUncachedAsync(IInterfaceLogger logger, int firstActiveVoltagePhase, bool singlePhase = false)
        => GetActualValuesAsync(logger, firstActiveVoltagePhase);

    /// <inheritdoc/>
    public abstract Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger);

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
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    protected static T GetRandomNumberWithDeviation<T>(T value, T deviation, T? min = default, T? max = default) where T : struct, IDomainSpecificNumber<T>
    {
        var maximum = value + deviation;
        var minimum = value - deviation;

        var diced = (Random.Shared.NextDouble() * (maximum - minimum)) + minimum;

        if (min.HasValue && diced < min.Value)
            return min.Value;

        if (max.HasValue && diced > max.Value)
            return max.Value;

        return diced;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="deviation"></param>
    /// <returns></returns>
    protected static T GetRandomNumberWithDeviation<T>(T value, double deviation) where T : struct, IDomainSpecificNumber<T>
        => GetRandomNumberWithDeviation(value, value * deviation / 100d);

    /// <inheritdoc/>
    public Task<Voltage[]> GetVoltageRangesAsync(IInterfaceLogger logger)
    {
        Voltage[] ranges =
        [
            new Voltage(420),
            new Voltage(250),
            new Voltage(125),
            new Voltage(60),
            new Voltage(5),
            new Voltage(0.25),
        ];

        return Task.FromResult(ranges);
    }

    /// <inheritdoc/>
    public Task<Current[]> GetCurrentRangesAsync(IInterfaceLogger logger)
    {
        Current[] ranges =
        [
            new Current(100),
            new Current(50),
            new Current(20),
            new Current(10),
            new Current(5),
            new Current(2),
            new Current(1),
            new Current(0.5),
            new Current(0.2),
            new Current(0.1),
            new Current(0.05),
            new Current(0.02),
        ];

        return Task.FromResult(ranges);
    }

    /// <inheritdoc/>
    public Task SetVoltageRangeAsync(IInterfaceLogger logger, Voltage voltage)
    {
        _refMeterStatus.VoltageRange = voltage;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SetCurrentRangeAsync(IInterfaceLogger logger, Current current)
    {
        _refMeterStatus.CurrentRange = current;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SetAutomaticAsync(IInterfaceLogger logger, bool voltageRanges = true, bool currentRanges = true, bool pll = true)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SelectPllChannelAsync(IInterfaceLogger logger, PllChannel pll)
    {
        _refMeterStatus.PllChannel = pll;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<RefMeterStatus> GetRefMeterStatusAsync(IInterfaceLogger logger)
    {
        return Task.FromResult(_refMeterStatus);
    }
}
