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

    /// <inheritdoc/>
    public async Task<MeasurementModes[]> GetMeasurementModes()
    {
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
        // Could be MI, but maybe not queryable
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        // Should be MA
        throw new NotImplementedException();
    }
}
