using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortFGRefMeter
{
    private static readonly Dictionary<string, MeasurementModes> SupportedModes = new() {
        {"1PHA", MeasurementModes.PhaseTApparentPower},
        {"1PHR", MeasurementModes.PhaseTReactivePower},
        {"1PHT", MeasurementModes.PhaseTPower},
        {"3BKB", MeasurementModes.ThreeWireReactivePowerCrossConnectedA},
        {"3LBE", MeasurementModes.ThreeWireReactivePower},
        {"3LBG", MeasurementModes.ThreeWireReactiveGeometricPower},
        {"3LBK", MeasurementModes.ThreeWireReactivePowerCrossConnectedB},
        {"3LQ6", MeasurementModes.ThreeWireReactive60Power},
        {"3LS", MeasurementModes.ThreeWireApparentPower},
        {"3LSG", MeasurementModes.ThreeWireApparentGeometricPower},
        {"3LW", MeasurementModes.ThreeWireActivePower},
        {"3LWR", MeasurementModes.ThreeWireSymmetricPower},
        {"3Q6K", MeasurementModes.ThreeWireReactive60SyntheticPower},
        {"4LBE", MeasurementModes.FourWireReactivePower},
        {"4LBF", MeasurementModes.FourWireReactiveRtsPower},
        {"4LBK", MeasurementModes.FourWireReactivePowerCrossConnected},
        {"4LDC", MeasurementModes.FourWireDCPower},
        {"4LQ6", MeasurementModes.FourWireReactive60Power},
        {"4LS", MeasurementModes.FourWireApparentPower},
        {"4LSG", MeasurementModes.FourWireApparentGeometricPower},
        {"4LW", MeasurementModes.FourWireActivePower},
        {"4Q6K", MeasurementModes.FourWireReactive60SyntheticPower},
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
