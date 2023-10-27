using System.Text.RegularExpressions;

using SerialPortProxy;

using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.Device;

/// <summary>
/// Handle all requests to a device.
/// </summary>
public class SerialPortSourceDevice : ISourceDevice
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly SerialPortConnection _device;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    public SerialPortSourceDevice(SerialPortConnection device)
    {
        _device = device;
    }

    /// <inheritdoc/>
    public async Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        /* Execute the request and wait for the information string. */
        var reply = await _device.Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

        if (reply.Length != 2)
            throw new InvalidOperationException($"wrong number of response lines - expected 2 but got {reply.Length}");

        /* Validate the response consisting of model name and version numner. */
        var versionMatch = _versionReg.Match(reply[0]);

        if (versionMatch?.Success != true)
            throw new InvalidOperationException($"invalid response {reply[0]} from device");

        /* Create response structure. */
        return new DeviceFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value,
            Version = versionMatch.Groups[2].Value
        };
    }
}
