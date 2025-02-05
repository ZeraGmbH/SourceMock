using BurdenApi.Models;
using RefMeterApi.Actions.Device;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions.Device;

/// <summary>
/// Simulation helper for the calibration of a burden.
/// </summary>
public class CalibrationHardwareMock : ICalibrationHardware
{
    private class BurdenMock : IBurdenMock
    {
        private readonly Dictionary<string, Calibration?> _calibrations = [];

        public void AddCalibration(string burden, string range, string step, Calibration? calibration)
        {
            lock (_calibrations)
                _calibrations[$"{burden};{range};{step}"] = calibration;
        }

        public bool IsAvailable => throw new NotImplementedException();

        public bool HasMockedSource => true;

        public Task CancelCalibrationAsync(IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task<string[]> GetBurdensAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task<Calibration?> GetCalibrationAsync(string burden, string range, string step, IInterfaceLogger interfaceLogger)
        {
            lock (_calibrations)
                return Task.FromResult(_calibrations.TryGetValue($"{burden};{range};{step}", out var calibration) ? calibration : null);
        }

        public Task<string[]> GetRangesAsync(string burden, IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task<BurdenStatus> GetStatusAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task<string[]> GetStepsAsync(string burden, IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task<BurdenVersion> GetVersionAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(new BurdenVersion());

        public Task<BurdenValues> MeasureAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task ProgramAsync(string? burden, IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task SetActiveAsync(bool on, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetBurdenAsync(string burden, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetPermanentCalibrationAsync(string burden, string range, string step, Calibration calibration, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetRangeAsync(string range, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetStepAsync(string step, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetTransientCalibrationAsync(Calibration calibration, IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task SetMeasuringCalibrationAsync(bool on, IInterfaceLogger interfaceLogger) => Task.CompletedTask;
    }

    private class RefMeterMock : IRefMeter
    {
        public Task<MeasurementModes?> GetActualMeasurementModeAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveCurrentPhase = -1) => throw new NotImplementedException();

        public Task<MeasuredLoadpoint> GetActualValuesUncachedAsync(IInterfaceLogger logger, int firstActiveCurrentPhase = -1, bool singlePhase = false) => throw new NotImplementedException();

        public Task<bool> GetAvailableAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<Current[]> GetCurrentRangesAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<MeasurementModes[]> GetMeasurementModesAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<MeterConstant> GetMeterConstantAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<RefMeterStatus> GetRefMeterStatusAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<Voltage[]> GetVoltageRangesAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task SelectPllChannelAsync(IInterfaceLogger logger, PllChannel pll) => throw new NotImplementedException();

        public Task SetActualMeasurementModeAsync(IInterfaceLogger logger, MeasurementModes mode) => Task.CompletedTask;

        public Task SetAutomaticAsync(IInterfaceLogger logger, bool voltageRanges = true, bool currentRanges = true, bool pll = true) => throw new NotImplementedException();

        public Task SetCurrentRangeAsync(IInterfaceLogger logger, Current current) => throw new NotImplementedException();

        public Task SetVoltageRangeAsync(IInterfaceLogger logger, Voltage voltage) => throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBurden Burden { get; } = new BurdenMock();

    /// <inheritdoc/>
    public IRefMeter ReferenceMeter { get; } = new RefMeterMock();

    private double? _CurrentRange;

    private ApparentPower? _CurrentPower;

    private readonly Calibration _Target = new(new(52, 113), new(119, 47));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="burden"></param>
    /// <param name="range"></param>
    /// <param name="step"></param>
    /// <param name="calibration"></param>
    public void AddCalibration(string burden, string range, string step, Calibration? calibration)
        => ((BurdenMock)Burden).AddCalibration(burden, range, step, calibration);

    /// <inheritdoc/>
    public Task<RefMeterValueWithQuantity> MeasureAsync(Calibration calibration, bool voltageNotCurrent)
    {
        if (!_CurrentRange.HasValue || !_CurrentPower.HasValue) throw new InvalidOperationException("loadpoint not yet set");

        var requestedResistance = CreateRelative(calibration.Resistive);
        var requestedImpedance = CreateRelative(calibration.Inductive);

        var resistance = (requestedResistance + 0.01d * requestedImpedance) / 1.01d;
        var impedance = 1 - (requestedImpedance + 0.01d * requestedResistance) / 1.01d;

        var targetResistance = CreateRelative(_Target.Resistive);

        return Task.FromResult(new RefMeterValueWithQuantity()
        {
            ApparentPower = resistance / targetResistance * _CurrentPower.Value,
            PowerFactor = new(impedance),
        });
    }

    private static double RelativeLimit => CreateRelative(127, 127);

    private static double CreateRelative(byte coarse, byte fine) => 128d * coarse + 1.1 * fine;

    private static double CreateRelative(CalibrationPair pair) => CreateRelative(pair.Coarse, pair.Fine) / RelativeLimit;

    /// <inheritdoc/>
    public Task<PrepareResult> PrepareAsync(bool voltageNotCurrent, string range, double percentage, Frequency frequency, bool detectRange, ApparentPower power, bool fixedPercentage = true)
    {
        var rangeValue = BurdenUtils.ParseRange(range);

        // Remember for emulation.
        _CurrentRange = percentage * rangeValue;
        _CurrentPower = power * percentage * percentage;

        return Task.FromResult(new PrepareResult
        {
            CurrentRange = voltageNotCurrent ? Current.Zero : new(rangeValue),
            Factor = percentage,
            IsVoltageNotCurrentBurden = voltageNotCurrent,
            VoltageRange = voltageNotCurrent ? new(rangeValue) : Voltage.Zero,
        });
    }

    /// <inheritdoc/>
    public Task<GoalValueWithQuantity> MeasureBurdenAsync(bool voltageNotCurrent) => Task.FromResult(new GoalValueWithQuantity());
}
