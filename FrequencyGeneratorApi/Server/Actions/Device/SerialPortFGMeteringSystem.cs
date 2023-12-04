using System.Text.RegularExpressions;
using MeteringSystemApi.Model;
using MeteringSystemApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace MeteringSystemApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class SerialPortFGMeteringSystem : IMeteringSystem
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^TS(.{8})(.{4})$", RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortFGMeteringSystem> _logger;

    private ISource _source = new UnavailableSource();

    private readonly IServiceProvider _services;

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
    /// <param name="services">Dependency injection system.</param>
    public SerialPortFGMeteringSystem(ISerialPortConnection device, ISerialPortFGRefMeter refMeter, ILogger<SerialPortFGMeteringSystem> logger, IServiceProvider services)
    {
        RefMeter = refMeter;

        _device = device;
        _logger = logger;
        _services = services;
    }

    /// <inheritdoc/>
    public Task<MeteringSystemCapabilities> GetCapabilities() =>
        Task.FromResult(new MeteringSystemCapabilities
        {
            SupportedCurrentAmplifiers = {
                CurrentAmplifiers.VI201x0x1,
                CurrentAmplifiers.VI201x01,
                CurrentAmplifiers.VI202x0x1,
                CurrentAmplifiers.VI202x0x2,
                CurrentAmplifiers.VI202x0x3,
                CurrentAmplifiers.VI202x0x4,
                CurrentAmplifiers.VI202x0x5,
                CurrentAmplifiers.VI202x0,
                CurrentAmplifiers.VI221x0,
                CurrentAmplifiers.VI222x0x1,
                CurrentAmplifiers.VI222x0,
            },
            SupportedReferenceMeters = {
                ReferenceMeters.COM3003x1x2,
                ReferenceMeters.COM3003x1x3,
                ReferenceMeters.COM3003,
                ReferenceMeters.COM5003x1x1,
                ReferenceMeters.COM5003x1,
                ReferenceMeters.EPZ303x10x1,
                ReferenceMeters.EPZ303x10,
                ReferenceMeters.EPZ303x8,
            },
            SupportedVoltageAmplifiers = {
                VoltageAmplifiers.VU211x012,
                VoltageAmplifiers.VU221x0x1,
                VoltageAmplifiers.VU221x0x2,
                VoltageAmplifiers.VU221x0x3,
                VoltageAmplifiers.VU221x0,
                VoltageAmplifiers.VU221x13,
                VoltageAmplifiers.VU221x2,
            }
        });

    /// <inheritdoc/>
    public async Task SetAmplifiersAndReferenceMeter(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters referenceMeter)
    {
        var capabilities = await GetCapabilities();

        if (!capabilities.SupportedVoltageAmplifiers.Contains(voltage))
            throw new ArgumentException(nameof(voltage));

        if (!capabilities.SupportedCurrentAmplifiers.Contains(current))
            throw new ArgumentException(nameof(current));

        if (!capabilities.SupportedReferenceMeters.Contains(referenceMeter))
            throw new ArgumentException(nameof(referenceMeter));

        var source = _services.GetRequiredService<ISerialPortFGSource>();

        source.SetAmplifiers(voltage, current);

        try
        {
            await _device.Execute(SerialPortRequest.Create($"ZP{(int)voltage:00}{(int)current:00}{(int)voltage:00}{(int)current:00}{(int)referenceMeter:00}", "OKZP"))[0];
        }
        catch (Exception)
        {
            source = null;

            throw;
        }
        finally
        {
            if (source != null)
                _source = source;
        }
    }

    /// <inheritdoc/>
    public async Task<MeteringSystemFirmwareVersion> GetFirmwareVersion()
    {
        /* Send command and check reply. */
        var reply = await _device.Execute(SerialPortRequest.Create("TS", _versionReg))[0];

        if (reply.Length < 1)
            throw new InvalidOperationException($"wrong number of response lines - expected at least 1 but got {reply.Length}");

        /* Validate the response consisting of model name and version numner. */
        var versionMatch = _versionReg.Match(reply[^1]);

        if (versionMatch?.Success != true)
            throw new InvalidOperationException($"invalid response {reply[0]} from device");

        /* Create response structure. */
        return new MeteringSystemFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value.Trim(),
            Version = versionMatch.Groups[2].Value.Trim()
        };
    }
}
