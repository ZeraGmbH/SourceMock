using BurdenApi.Models;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions.Device;

/// <summary>
/// Implementation of a calibration environment.
/// </summary>
public class CalibrationHardware(ISource source, IRefMeter refMeter, IBurden burden, IInterfaceLogger logger) : ICalibrationHardware
{
    /// <inheritdoc/>
    public IBurden Burden { get; } = burden;

    private Voltage _VoltageRange;

    private Current _CurrentRange;

    /// <inheritdoc/>
    public async Task<Tuple<GoalValue, double?>> MeasureAsync(Calibration calibration, bool voltageNotCurrent)
    {
        // Apply the calibration to the burden.        
        await Burden.SetTransientCalibrationAsync(calibration, logger);

        // Check for simulation mode to speed up tests.
        var isMock = Burden is IBurdenMock mockedBurden && mockedBurden.HasMockedSource;

        // Relax a bit.
        if (!isMock) await Task.Delay(5000);

        for (var retry = 3; retry-- > 0;)
        {
            // Ask device for values.
            var values = await refMeter.GetActualValuesUncachedAsync(logger, -1, true);

            // Check for keeping the ranges.
            if ((double)_VoltageRange != 0 && (double)_CurrentRange != 0)
            {
                // Ask for current ranges.
                var status = await refMeter.GetRefMeterStatusAsync(logger);

                if (status.VoltageRange != _VoltageRange || status.CurrentRange != _CurrentRange)
                {
                    if (status.VoltageRange != _VoltageRange)
                        await refMeter.SetVoltageRangeAsync(logger, _VoltageRange);

                    if (status.CurrentRange != _CurrentRange)
                        await refMeter.SetCurrentRangeAsync(logger, _CurrentRange);

                    continue;
                }
            }

            // Report.
            var power = values.Phases[0].ApparentPower;
            var factor = values.Phases[0].PowerFactor;

            if (power == null || factor == null) throw new InvalidOperationException("insufficient actual values");

            return Tuple.Create(
                new GoalValue()
                {
                    ApparentPower = power.Value,
                    PowerFactor = factor.Value
                },
                voltageNotCurrent
                    ? (double?)values.Phases[0].Voltage.AcComponent?.Rms
                    : (double?)values.Phases[0].Current.AcComponent?.Rms);
        }

        throw new InvalidOperationException("reference meter keeps changing ranges - unable to get reliable actual values");
    }


    /// <inheritdoc/>
    public async Task<Tuple<double, double>> PrepareAsync(bool voltageNotCurrent, string range, double percentage, Frequency frequency, bool detectRange, ApparentPower power, bool fixedPercentage = true)
    {
        // Get the capabilities from the source.
        var caps = await source.GetCapabilitiesAsync(logger);

        var minRange = voltageNotCurrent ? (double?)caps.Phases[0].AcVoltage?.Min : (double?)caps.Phases[0].AcCurrent?.Min;
        var maxRange = voltageNotCurrent ? (double?)caps.Phases[0].AcVoltage?.Max : (double?)caps.Phases[0].AcCurrent?.Max;

        // Analyse the range pattern.
        var rawValue = BurdenUtils.ParseRange(range);
        var rangeValue = percentage * rawValue;

        if (!fixedPercentage && minRange.HasValue && maxRange.HasValue)
            if (rangeValue < minRange.Value)
            {
                rangeValue = minRange.Value;
                percentage = rangeValue / rawValue;
            }
            else if (rangeValue > maxRange.Value)
            {
                rangeValue = maxRange.Value;
                percentage = rangeValue / rawValue;
            }

        // Get the scaling factors and use the best fit value in the allowed precision range.
        var stepSize = caps.Phases.Count < 1
            ? null
            : voltageNotCurrent
            ? (double?)caps.Phases[0].AcVoltage?.PrecisionStepSize
            : (double?)caps.Phases[0].AcCurrent?.PrecisionStepSize;

        var stepFactor = stepSize ?? 0;

        if (stepFactor > 0)
            rangeValue = stepFactor * Math.Round(rangeValue / stepFactor);

        // Create the IEC loadpoint.
        var lp = new TargetLoadpoint
        {
            Frequency = { Mode = FrequencyMode.SYNTHETIC, Value = frequency },
            Phases = {
                new() {
                    Current = new() { On = !voltageNotCurrent, AcComponent = new() { Rms = new(voltageNotCurrent ? 0 : rangeValue)}  } ,
                    Voltage = new() { On = voltageNotCurrent, AcComponent = new() { Rms = new(voltageNotCurrent ? rangeValue: 0)} }
                },
                new() { Current = new() { On = false, AcComponent = new() }, Voltage = new() { On = false, AcComponent = new() } },
                new() { Current = new() { On = false, AcComponent = new() }, Voltage = new() { On = false, AcComponent = new() } },
        }
        };

        // Set the loadpioint.
        var status = await source.SetLoadpointAsync(logger, lp);

        if (status != SourceApiErrorCodes.SUCCESS) throw new InvalidOperationException($"bad loadpoint: {status}");

        // Check for simulation mode to speed up tests.
        var isMock = Burden is IBurdenMock mockedBurden && mockedBurden.HasMockedSource;

        // Wait a bit for stabilisation.
        if (!isMock) await Task.Delay(5000);

        // Get the best fit on reference meter range.
        if (detectRange)
        {
            // Use manual.
            await refMeter.SetAutomaticAsync(logger, false, false, false);

            // Get all the supported ranges.
            var currentRanges = await refMeter.GetCurrentRangesAsync(logger);
            var voltageRanges = await refMeter.GetVoltageRangesAsync(logger);

            if (currentRanges.Length < 1) throw new InvalidOperationException("no reference meter current ranges found");
            if (voltageRanges.Length < 1) throw new InvalidOperationException("no reference meter voltage ranges found");

            Array.Sort(currentRanges);
            Array.Sort(voltageRanges);

            // Calculate the current from the apparent power and the voltage.
            var otherRange = (double)(power * percentage * percentage) / rangeValue;
            var voltageRange = voltageNotCurrent ? rangeValue : otherRange;
            var currentRange = voltageNotCurrent ? otherRange : rangeValue;

            // Find the first bigger ones.
            var currentIndex = Array.FindIndex(currentRanges, r => (double)r >= currentRange);
            var voltageIndex = Array.FindIndex(voltageRanges, r => (double)r >= voltageRange);

            // Use the ranges.            
            _VoltageRange = voltageIndex < 0 ? voltageRanges[^1] : voltageRanges[voltageIndex];
            _CurrentRange = currentIndex < 0 ? currentRanges[^1] : currentRanges[currentIndex];

            await refMeter.SetVoltageRangeAsync(logger, _VoltageRange);
            await refMeter.SetCurrentRangeAsync(logger, _CurrentRange);
        }
        else
        {
            // Use automatic.
            _VoltageRange = Voltage.Zero;
            _CurrentRange = Current.Zero;

            await refMeter.SetAutomaticAsync(logger, true, true, false);
        }

        // Choose the PLL channel - put in manual mode before.
        await refMeter.SelectPllChannelAsync(logger, voltageNotCurrent ? PllChannel.U1 : PllChannel.I1);

        // Configure reference meter.
        await refMeter.SetActualMeasurementModeAsync(logger, MeasurementModes.MqBase);

        // Report new pecentage
        return Tuple.Create(percentage, rangeValue);
    }

    /// <inheritdoc/>
    public async Task<Tuple<GoalValue, double?>> MeasureBurdenAsync(bool voltageNotCurrent)
    {
        var values = await Burden.MeasureAsync(logger);

        return Tuple.Create(
             new GoalValue(values.ApparentPower, values.PowerFactor),
             voltageNotCurrent ? (double?)values.Voltage : (double?)values.Current
        );
    }
}