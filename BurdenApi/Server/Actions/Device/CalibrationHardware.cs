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
    public async Task<GoalValue> MeasureAsync(Calibration calibration)
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

            return new() { ApparentPower = power.Value, PowerFactor = factor.Value };
        }

        throw new InvalidOperationException("reference meter keeps changing ranges - unable to get reliable actual values");
    }


    /// <inheritdoc/>
    public async Task PrepareAsync(string range, double percentage, Frequency frequency, bool detectRange, ApparentPower power)
    {
        // Analyse the range pattern.
        var rangeValue = percentage * BurdenUtils.ParseRange(range);

        // Check the type of burden.
        var burdenInfo = await Burden.GetVersionAsync(logger);

        // Create the IEC loadpoint.
        var lp = new TargetLoadpoint
        {
            Frequency = { Mode = FrequencyMode.SYNTHETIC, Value = frequency },
            Phases = {
                new() {
                    Current = new() { On = !burdenInfo.IsVoltageNotCurrent, AcComponent = new() { Rms = new(rangeValue)}  } ,
                    Voltage = new() { On = burdenInfo.IsVoltageNotCurrent, AcComponent = new() { Rms = new(rangeValue)} }
                },
                new() { Current = new() { On = false, AcComponent = new() }, Voltage = new() { On = false, AcComponent = new() } },
                new() { Current = new() { On = false, AcComponent = new() }, Voltage = new() { On = false, AcComponent = new() } },
        }
        };

        // Set the loadpioint.
        var status = await source.SetLoadpointAsync(logger, lp);

        if (status != SourceApiErrorCodes.SUCCESS) throw new InvalidOperationException("bad loadpoint");

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
            var voltageRange = burdenInfo.IsVoltageNotCurrent ? rangeValue : otherRange;
            var currentRange = burdenInfo.IsVoltageNotCurrent ? otherRange : rangeValue;

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
        await refMeter.SelectPllChannelAsync(logger, burdenInfo.IsVoltageNotCurrent ? PllChannel.U1 : PllChannel.I1);

        // Configure reference meter.
        await refMeter.SetActualMeasurementModeAsync(logger, MeasurementModes.MqBase);
    }

    /// <inheritdoc/>
    public async Task<GoalValue> MeasureBurdenAsync()
    {
        var values = await Burden.MeasureAsync(logger);

        return new(values.ApparentPower, values.PowerFactor);
    }
}