using System.Text.RegularExpressions;
using BurdenApi.Models;
using RefMeterApi.Actions.Device;
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
    private static readonly Regex _RangePattern = new("^([^/]+)(/(3|v3))?$");

    /// <inheritdoc/>
    public Task<GoalValue> MeasureAsync(Calibration calibration)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task SetLoadpointAsync(string range, double percentage, Frequency frequency, bool detectRange, PowerFactor powerFactor)
    {
        // Analyse the range pattern - assume some number optional followed by a scaling.
        var match = _RangePattern.Match(range);

        if (!match.Success) throw new ArgumentException(range, nameof(range));

        var rangeValue = percentage * double.Parse(match.Groups[1].Value);

        switch (match.Groups[3].Value)
        {
            case "3":
                rangeValue /= 3;
                break;
            case "v3":
                rangeValue /= Math.Sqrt(3);
                break;
        }

        // Change power factor to angle.
        var angle = (Angle)powerFactor;

        // Check the type of burden.
        var burdenInfo = await burden.GetVersionAsync(logger);
        var isVoltageNotCurrent = burdenInfo.IsVoltageNotCurrent;

        // Create the IEC loadpoint.
        var lp = new TargetLoadpoint
        {
            Frequency = { Mode = FrequencyMode.SYNTHETIC, Value = frequency },
            Phases = {
                new() {
                    Current = new() {
                        On = !isVoltageNotCurrent,
                        AcComponent = new() { Rms = new(isVoltageNotCurrent ? 0 : rangeValue), Angle = Angle.Zero }
                    },
                    Voltage = new() {
                        On = isVoltageNotCurrent,
                        AcComponent =  new() { Rms = new(isVoltageNotCurrent ? rangeValue : 0), Angle = (new Angle(360) - angle).Normalize() }
                    }
                },
                new() { Current = new() { On = false }, Voltage = new() { On = false } },
                new() { Current = new() { On = false }, Voltage = new() { On = false } },
            }
        };

        // Set the loadpioint.
        var status = await source.SetLoadpointAsync(logger, lp);

        if (status != SourceApiErrorCodes.SUCCESS) throw new InvalidOperationException("bad loadpoint");

        // Get the best fit on reference meter range.
        if (detectRange)
        {
            // Get all the supported ranges.
            var ranges = (isVoltageNotCurrent
                ? (await refMeter.GetVoltageRangesAsync(logger)).Select(v => (double)v)
                : (await refMeter.GetCurrentRangesAsync(logger)).Select(c => (double)c)).Order().ToList();

            if (ranges.Count < 1) throw new InvalidOperationException("no reference meter ranges found");

            // Find the first bigger one.
            var index = ranges.FindIndex(r => r >= rangeValue);
            var refMeterRange = index < 0 ? ranges[^1] : ranges[index];

            // Use the range.
            if (isVoltageNotCurrent)
                await refMeter.SetVoltageRangeAsync(logger, new(refMeterRange));
            else
                await refMeter.SetCurrentRangeAsync(logger, new(refMeterRange));
        }
    }
}