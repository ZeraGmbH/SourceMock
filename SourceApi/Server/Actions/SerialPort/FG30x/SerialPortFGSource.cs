using System.Text.RegularExpressions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public class SerialPortFGSource : ISource
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^TS(.{8})(.{4})$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly ILogger<SerialPortFGSource> _logger;

    private readonly SerialPortConnection _device;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    public SerialPortFGSource(ILogger<SerialPortFGSource> logger, SerialPortConnection device)
    {
        _device = device;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task CancelDosage()
    {
        // 290 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo()
    {
        // Software only
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities()
    {
        // Software only
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Loadpoint? GetCurrentLoadpoint()
    {
        // Software only
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgress()
    {
        // 243/252 or 3SA/3MA
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        /* Send command and check reply. */
        var reply = await _device.Execute(SerialPortRequest.Create("TS", _versionReg))[0];

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
    public Task SetDosageEnergy(double value)
    {
        // 210/211 or 3PS
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetDosageMode(bool on)
    {
        // 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        // FR, IP, UP, UI
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task StartDosage()
    {
        // 242 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceResult> TurnOff()
    {
        // 2S0/2S1
        throw new NotImplementedException();
    }
}
