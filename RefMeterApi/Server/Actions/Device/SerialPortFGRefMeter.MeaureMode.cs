using System.Text.RegularExpressions;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortFGRefMeter
{
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
    };

    private static readonly Regex MiCommand = new(@"^MI([^;]+;)*$");

    private MeasurementModes? _measurementMode = null;

    /// <inheritdoc/>
    public async Task<MeasurementModes[]> GetMeasurementModes()
    {
        TestConfigured();

        /* Request from device. */
        var replies = await _device.Execute(SerialPortRequest.Create("MI", MiCommand))[0];

        /* Analyse result and report all supported values. */
        var response = new List<MeasurementModes>();

        foreach (var rawMode in replies[^1][2..^1].Split(';'))
            if (SupportedModes.TryGetValue(rawMode, out var mode))
                response.Add(mode);

        return response.ToArray();
    }

    /// <inheritdoc/>
    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        TestConfigured();


        return Task.FromResult(_measurementMode);
    }

    /// <inheritdoc/>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        TestConfigured();

        /* Reverse lookup the raw string for the mode - somewhat slow. */
        var supported = SupportedModes.Single(m => m.Value == mode);

        /* Send the command to the device. */
        return _device
            .Execute(SerialPortRequest.Create($"MA{supported.Key}", "OKMA"))[0]
            .ContinueWith((_t) => _measurementMode = mode);
    }
}
