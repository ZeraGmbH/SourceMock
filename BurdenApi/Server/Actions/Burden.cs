using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions;

/// <summary>
/// Implementation of the burden communication interface.
/// </summary>
/// <param name="device">Serial port connection to the hardware.</param>
public class Burden([FromKeyedServices("Burden")] ISerialPortConnection device) : IBurden
{
    /// <inheritdoc/>
    public async Task<string[]> GetBurdensAsync(IInterfaceLogger log)
    {
        // Request version from device.
        var burdens = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create("AB", "ABACK"))[0];

        if (burdens.Length < 1) throw new InvalidOperationException($"to few resonse lines");

        return [.. burdens.Take(burdens.Length - 1)];
    }

    /// <inheritdoc/>
    public async Task<Calibration?> GetCalibrationAsync(string burden, string range, string step, IInterfaceLogger log)
    {
        // Request version from device.
        var values = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"GA{burden};{range};{step}", "GAACK"))[0];

        if (values.Length < 2) throw new InvalidOperationException($"to few resonse lines");

        // Reconstruct the calibration data.
        return Calibration.Parse(values[^2]);
    }

    /// <inheritdoc/>
    public async Task<BurdenVersion> GetVersionAsync(IInterfaceLogger log)
    {
        // Request version from device.
        var version = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create("AV", "AVACK"))[0];

        if (version.Length < 3) throw new InvalidOperationException($"to few resonse lines");

        // Create reply.
        return new() { Version = version[^3], Supplement = version[^2] };
    }

    /// <inheritdoc/>
    public Task ProgramAsync(string? burden, IInterfaceLogger log)
        // Enforce a read timeout of 20 Minutes - in worst case serial line will be blocked for that duration.
        => device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"PR{burden ?? string.Empty}", "PRACK", 1200000))[0];

    /// <inheritdoc/>
    public Task SetActiveAsync(bool on, IInterfaceLogger log)
        => device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"ON{(on ? 1 : 0)}", "ONACK"))[0];
}
