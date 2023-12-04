using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SerialPortProxy;

using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public class SerialPortFGSource : CommonSource<FGLoadpointTranslator>
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^TS(.{8})(.{4})$", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    public SerialPortFGSource(ILogger<SerialPortFGSource> logger, ISerialPortConnection device) : base(logger, device)
    {
    }

    /// <inheritdoc/>
    public override Task CancelDosage()
    {
        // 290 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<SourceCapabilities> GetCapabilities()
    {
        /* Currently we assume MT768, future versions may read the firmware from the device. */
        return Task.FromResult(CapabilitiesMap.GetCapabilitiesByModel("MT786"));
    }

    /// <inheritdoc/>
    public override Task<DosageProgress> GetDosageProgress()
    {
        // 243/252 or 3SA/3MA
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override async Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        /* Send command and check reply. */
        var reply = await Device.Execute(SerialPortRequest.Create("TS", _versionReg))[0];

        if (reply.Length < 1)
            throw new InvalidOperationException($"wrong number of response lines - expected 2 but got {reply.Length}");

        /* Validate the response consisting of model name and version numner. */
        var versionMatch = _versionReg.Match(reply[^1]);

        if (versionMatch?.Success != true)
            throw new InvalidOperationException($"invalid response {reply[0]} from device");

        /* Create response structure. */
        return new DeviceFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value.Trim(),
            Version = versionMatch.Groups[2].Value.Trim()
        };
    }

    /// <inheritdoc/>
    public override Task SetDosageEnergy(double value)
    {
        // 210/211 or 3PS
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(bool on)
    {
        // 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task StartDosage()
    {
        // 242 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<SourceResult> TurnOff()
    {
        // 2S0/2S1
        throw new NotImplementedException();
    }
}
