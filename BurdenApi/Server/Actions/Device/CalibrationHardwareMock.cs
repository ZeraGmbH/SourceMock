using BurdenApi.Models;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions.Device;

/// <summary>
/// Simulation helper for the calibration of a burden.
/// </summary>
public class CalibrationHardwareMock : ICalibrationHardware
{
    private class BurdenMock : IBurden
    {
        private readonly Dictionary<string, Calibration?> _calibrations = [];

        public void AddCalibration(string burden, string range, string step, Calibration? calibration)
        {
            lock (_calibrations)
                _calibrations[$"{burden};{range};{step}"] = calibration;
        }

        public bool IsAvailable => throw new NotImplementedException();

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

        public Task<BurdenVersion> GetVersionAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(new BurdenVersion { IsVoltageNotCurrent = true });

        public Task<BurdenValues> MeasureAsync(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task ProgramAsync(string? burden, IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

        public Task SetActiveAsync(bool on, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetBurdenAsync(string burden, IInterfaceLogger interfaceLogger) => Task.CompletedTask;

        public Task SetPermanentCalibrationAsync(string burden, string range, string step, Calibration calibration, IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

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

    private const int CorrelationLimit = 127 * 128 + 127;

    private const double FactorLimit = (CorrelationLimit + CorrelationLimit / 1000d) / 123d;

    /// <inheritdoc/>
    public IBurden Burden { get; } = new BurdenMock();

    /// <inheritdoc/>
    public IRefMeter ReferenceMeter { get; } = new RefMeterMock();

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
    public Task<GoalValue> MeasureAsync(Calibration calibration)
    {
        var resCalibration = calibration.Resistive.Coarse * 128d + calibration.Resistive.Fine;
        var indCalibration = calibration.Inductive.Coarse * 128d + calibration.Inductive.Fine;

        var resistence = resCalibration + indCalibration / 1000d;
        var inductive = indCalibration + resCalibration / 1000d;

        var power = resistence / 123d;
        var factor = inductive / 123d;

        return Task.FromResult(new GoalValue(new(power), new(factor / FactorLimit)));
    }

    /// <inheritdoc/>
    public Task PrepareAsync(string range, double percentage, Frequency frequency, bool detectRange, GoalValue goal)
    {
        return Task.CompletedTask;
    }
}
