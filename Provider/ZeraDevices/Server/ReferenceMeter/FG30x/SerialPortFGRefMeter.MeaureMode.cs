using System.Text.RegularExpressions;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace ZeraDevices.ReferenceMeter.MeterConstantCalculator.FG30x;

partial class SerialPortFGRefMeter
{
    /// <summary>
    /// All supported measurement modes - this is a minor restriction on what
    /// e.g. the FG30x supports.
    /// </summary>
    private static readonly Dictionary<string, MeasurementModes> SupportedModes = new() {
        {"3BKB", MeasurementModes.ThreeWireReactivePowerCrossConnectedA},
        {"3LBE", MeasurementModes.ThreeWireReactivePower},
        {"3LBK", MeasurementModes.ThreeWireReactivePowerCrossConnectedB},
        {"3LS", MeasurementModes.ThreeWireApparentPower},
        {"3LW", MeasurementModes.ThreeWireActivePower},
        {"4LBE", MeasurementModes.FourWireReactivePower},
        {"4LBK", MeasurementModes.FourWireReactivePowerCrossConnected},
        {"4LS", MeasurementModes.FourWireApparentPower},
        {"4LW", MeasurementModes.FourWireActivePower},
        {"MQBase", MeasurementModes.MqBase},
        {"MQRef", MeasurementModes.MqRef},
    };

    /// <summary>
    /// The current measurement mode - the device will not report the setting
    /// so we have to remember it here./// </summary>
    private MeasurementModes? _measurementMode = null;

    /// <inheritdoc/>
    public async Task<MeasurementModes[]> GetMeasurementModesAsync(IInterfaceLogger logger)
    {
        await TestConfiguredAsync(logger);

        /* Request from device. */
        var replies = await _device.ExecuteAsync(logger, SerialPortRequest.Create("MI", new Regex(@"^MI([^;]+;)*$")))[0];

        /* Analyse result and report all supported values. */
        var response = new List<MeasurementModes>();

        foreach (var rawMode in replies[^1][2..^1].Split(';'))
            if (SupportedModes.TryGetValue(rawMode, out var mode))
                response.Add(mode);

        return [.. response];
    }

    /// <inheritdoc/>
    public async Task<MeasurementModes?> GetActualMeasurementModeAsync(IInterfaceLogger logger)
    {
        await TestConfiguredAsync(logger);

        /* We can only report what was last set using the interface. */
        return _measurementMode;
    }

    /// <inheritdoc/>
    public async Task SetActualMeasurementModeAsync(IInterfaceLogger logger, MeasurementModes mode)
    {
        await TestConfiguredAsync(logger);

        /* Reverse lookup the raw string for the mode - somewhat slow. */
        var supported = SupportedModes.Single(m => m.Value == mode);

        /* Send the command to the device. */
        await _device
            .ExecuteAsync(logger, SerialPortRequest.Create($"MA{supported.Key}", "OKMA"))[0]
            .ContinueWith((_t) => _measurementMode = mode, TaskScheduler.Current);
    }
}
