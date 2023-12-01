using FrequencyGeneratorApi.Models;
using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace FrequencyGeneratorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class SerialPortFGFrequencyGenerator : IFrequencyGenerator
{
    private readonly SerialPortConnection _device;

    private readonly ILogger<SerialPortFGFrequencyGenerator> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortFGFrequencyGenerator(SerialPortConnection device, ILogger<SerialPortFGFrequencyGenerator> logger)
    {
        _device = device;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<FrequencyGeneratorCapabilities> GetCapabilities() =>
        Task.FromResult(new FrequencyGeneratorCapabilities
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

        await _device.Execute(SerialPortRequest.Create($"ZP{(int)voltage:00}{(int)current:00}{(int)voltage:00}{(int)current:00}{(int)referenceMeter:00}", "OKZP"))[0];
    }
}
