using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using SerialPortProxy;
using SharedLibrary.Models.Logging;

namespace RefMeterApi.Actions.Device;

partial class SerialPortMTRefMeter
{
    /// <summary>
    /// All currently supported measurement modes - the meter test system
    /// may provide additional modes.
    /// </summary>
    private static readonly Dictionary<string, MeasurementModes> SupportedModes = new() {
        {"2WA", MeasurementModes.TwoWireActivePower},
        {"2WAP", MeasurementModes.TwoWireApparentPower},
        {"2WR", MeasurementModes.TwoWireReactivePower},
        {"3WA", MeasurementModes.ThreeWireActivePower},
        {"3WAP", MeasurementModes.ThreeWireApparentPower},
        {"3WR", MeasurementModes.ThreeWireReactivePower},
        {"3WRCA", MeasurementModes.ThreeWireReactivePowerCrossConnectedA},
        {"3WRCB", MeasurementModes.ThreeWireReactivePowerCrossConnectedB},
        {"4WA", MeasurementModes.FourWireActivePower},
        {"4WAP", MeasurementModes.FourWireApparentPower},
        {"4WR", MeasurementModes.FourWireReactivePower},
        {"4WRC", MeasurementModes.FourWireReactivePowerCrossConnected},
    };

    /// <summary>
    /// Pattern to recognize a measurement mode reported by the device.
    /// </summary>
    private static readonly Regex MeasurementModeReg = new Regex(@"^(\d{1,3});([^;]+);(.+)$");

    /// <inheritdoc/>
    public async Task<MeasurementModes?> GetActualMeasurementMode(IInterfaceLogger logger)
    {
        /* Execute the request and get the answer from the device. */
        var replies = await _device.Execute(logger, SerialPortRequest.Create("AST", "ASTACK"))[0];

        /* Find the mode. */
        var modeAsString = replies.SingleOrDefault(r => r.StartsWith("M="));

        if (modeAsString == null)
            return null;

        return SupportedModes.TryGetValue(modeAsString[2..], out var mode) ? mode : null;
    }

    /// <inheritdoc/>
    public async Task<MeasurementModes[]> GetMeasurementModes(IInterfaceLogger logger)
    {
        /* Execute the request and get the answer from the device. */
        var replies = await _device.Execute(logger, SerialPortRequest.Create("AML", "AMLACK"))[0];

        /* Prepare response with three phases. */
        var response = new List<MeasurementModes>();

        for (var i = 0; i < replies.Length - 1; i++)
        {
            /* Chck for a value with index. */
            var reply = replies[i];
            var match = MeasurementModeReg.Match(reply);

            if (!match.Success)
            {
                /* Report bad reply and ignore it. */
                _logger.LogWarning($"bad reply {reply}");

                continue;
            }

            /* Get the english short name. */
            if (SupportedModes.TryGetValue(match.Groups[2].Value, out var mode))
                response.Add(mode);
        }

        return response.ToArray();
    }


    /// <inheritdoc/>
    public Task SetActualMeasurementMode(IInterfaceLogger logger, MeasurementModes mode)
    {
        /* Reverse lookup the raw string for the mode - somewhat slow. */
        var supported = SupportedModes.Single(m => m.Value == mode);

        /* Send the command to the device. */
        return _device.Execute(logger, SerialPortRequest.Create($"AMT{supported.Key}", "AMTACK"))[0];
    }
}
