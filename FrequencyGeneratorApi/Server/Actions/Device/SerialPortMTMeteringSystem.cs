using System.Text.RegularExpressions;
using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Model;
using MeterTestSystemApi.Models;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions.Device;
using SerialPortProxy;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class SerialPortMTMeterTestSystem : IMeterTestSystem
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortMTMeterTestSystem> _logger;

    private readonly ISerialPortMTSource _source;

    /// <inheritdoc/>
    public ISource Source => _source;

    /// <inheritdoc/>
    public IRefMeter RefMeter { get; private set; }

    /// <inheritdoc/>
    public IErrorCalculator ErrorCalculator { get; private set; }

    /// <inheritdoc/>
    public AmplifiersAndReferenceMeters AmplifiersAndReferenceMeters => throw new NotImplementedException();

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="refMeter">The related reference meter.</param>
    /// <param name="errorCalculator">The error calculator of this metering system.</param>
    /// <param name="logger">Logging service for this device type.</param>
    /// <param name="source">Source to use to access the metering system.</param>
    public SerialPortMTMeterTestSystem(ISerialPortConnection device, ISerialPortMTRefMeter refMeter, ISerialPortMTErrorCalculator errorCalculator, ILogger<SerialPortMTMeterTestSystem> logger, ISerialPortMTSource source)
    {
        ErrorCalculator = errorCalculator;
        RefMeter = refMeter;

        _device = device;
        _logger = logger;
        _source = source;
    }

    /// <inheritdoc/>
    public async Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion()
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
        return new MeterTestSystemFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value,
            Version = versionMatch.Groups[2].Value
        };
    }

    /// <inheritdoc/>
    public Task<MeterTestSystemCapabilities> GetCapabilities() => Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeters settings)
    {
        throw new NotImplementedException("SetAmplifiersAndReferenceMeter");
    }
}
