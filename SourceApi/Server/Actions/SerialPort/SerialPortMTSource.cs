using System.Text.RegularExpressions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// A ISource implenmentation to access a (potentially mocked) device. This
/// should be a singleton because it manages the loadpoint.
/// </summary>
public partial class SerialPortMTSource : ISource
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly ILogger<SerialPortMTSource> _logger;

    private readonly SerialPortConnection _device;

    private readonly LoadpointInfo _info = new();

    /// <summary>
    /// Initialize a new source implementation.
    /// </summary>
    /// <param name="logger">Logger to use.</param>
    /// <param name="device">Access to the serial port.</param>
    public SerialPortMTSource(ILogger<SerialPortMTSource> logger, SerialPortConnection device)
    {
        _device = device;
        _logger = logger;
    }

    private Loadpoint? _loadpoint;

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities()
    {
        /* Currently we assume MT768, future versions may read the firmware from the device. */
        return Task.FromResult(CapabilitiesMap.GetCapabilitiesByModel("MT786"));
    }

    /// <inheritdoc/>
    public Loadpoint? GetCurrentLoadpoint() => _loadpoint;

    /// <inheritdoc/>
    public async Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        /* Always validate the loadpoint against the device capabilities. */
        var isValid = SourceCapabilityValidator.IsValid(loadpoint, await GetCapabilities());

        if (isValid != SourceResult.SUCCESS)
            return isValid;

        _logger.LogTrace("Loadpoint set, source turned on.");

        /* Remember loadpoint even if we were not able to completly send it to the device. */
        _loadpoint = loadpoint;
        _info.SavedAt = DateTime.Now;

        try
        {
            await Task.WhenAll(_device.Execute(LoadpointTranslator.ToSerialPortRequests(loadpoint)));

            _info.ActivatedAt = DateTime.Now;
        }
        catch (Exception e)
        {
            /* At least one request in the transaction failed, transaction was aborted and device not turned on - this would be the last request in the transaction. */
            _logger.LogWarning("Loadpoint set, but source could not be turned on: {0}", e);

            return SourceResult.SUCCESS_NOT_ACTIVATED;
        }

        return SourceResult.SUCCESS;
    }

    /// <inheritdoc/>
    public async Task<SourceResult> TurnOff()
    {
        _logger.LogTrace("Switching anything off.");

        await Task.WhenAll(_device.Execute(SerialPortRequest.Create("SUIAAAAAAAAA", "SOKUI")));

        return SourceResult.SUCCESS;
    }

    /// <inheritdoc/>
    public async Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        /* Execute the request and wait for the information string. */
        var reply = await _device.Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

        if (reply.Length < 2)
            throw new InvalidOperationException($"wrong number of response lines - expected 2 but got {reply.Length}");

        /* Validate the response consisting of model name and version numner. */
        var versionMatch = _versionReg.Match(reply[^2]);

        if (versionMatch?.Success != true)
            throw new InvalidOperationException($"invalid response {reply[0]} from device");

        /* Create response structure. */
        return new DeviceFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value,
            Version = versionMatch.Groups[2].Value
        };
    }

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo() => _info;

}
