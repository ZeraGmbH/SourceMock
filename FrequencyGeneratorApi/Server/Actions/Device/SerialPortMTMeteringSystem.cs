using System.Text.RegularExpressions;
using MeteringSystemApi.Model;
using MeteringSystemApi.Models;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace MeteringSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class SerialPortMTMeteringSystem : IMeteringSystem
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortMTMeteringSystem> _logger;

    private readonly ISerialPortMTSource _source;

    /// <inheritdoc/>
    public ISource Source => _source;

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; private set; }

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="refMeter">The related reference meter.</param>
    /// <param name="logger">Logging service for this device type.</param>
    /// <param name="source">Source to use to access the metering system.</param>
    public SerialPortMTMeteringSystem(ISerialPortConnection device, ISerialPortMTRefMeter refMeter, ILogger<SerialPortMTMeteringSystem> logger, ISerialPortMTSource source)
    {
        RefMeter = refMeter;

        _device = device;
        _logger = logger;
        _source = source;
    }

    /// <inheritdoc/>
    public async Task<MeteringSystemFirmwareVersion> GetFirmwareVersion()
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
        return new MeteringSystemFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value,
            Version = versionMatch.Groups[2].Value
        };
    }

    /// <inheritdoc/>
    public Task<MeteringSystemCapabilities> GetCapabilities() => Task.FromResult<MeteringSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters referenceMeter)
    {
        throw new NotImplementedException("SetAmplifiersAndReferenceMeter");
    }
}
