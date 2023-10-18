using System.Text.RegularExpressions;

using SerialPortProxy;

using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.Device;

/// <summary>
/// Handle all requests to a device.
/// </summary>
public class SerialPortDevice : IDevice
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly SerialPortService _service;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="service">Service to access the current serial port.</param>
    public SerialPortDevice(SerialPortService service)
    {
        _service = service;
    }

    /// <inheritdoc/>
    public async Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        /* Execute the request and wait for the information string. */
        var reply = await _service.Execute(SerialPortRequest.Create("AAV", "AAVACK"))[0];

        if (reply.Length != 2)
            throw new InvalidOperationException("too many response lines");

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
