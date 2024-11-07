using BurdenApi.Models;
using Microsoft.AspNetCore.Mvc;
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
        // Request burdens from device.
        var burdens = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create("AB", "ABACK"))[0];

        if (burdens.Length < 1) throw new InvalidOperationException($"too few response lines");

        return [.. burdens.Take(burdens.Length - 1)];
    }

    private async Task<Tuple<Calibration, string>?> GetCalibrationInfoAsync(string burden, string range, string step, IInterfaceLogger log)
    {
        // Request values from device.
        var values = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"GA{burden};{range};{step}", "GAACK"))[0];

        if (values.Length < 2) throw new InvalidOperationException($"too few response lines");

        // Reconstruct the calibration data.
        return Calibration.Parse(values[^2]);
    }

    /// <inheritdoc/>
    public async Task<Calibration?> GetCalibrationAsync(string burden, string range, string step, IInterfaceLogger log)
        => (await GetCalibrationInfoAsync(burden, range, step, log))?.Item1;

    /// <inheritdoc/>
    public async Task<BurdenStatus> GetStatusAsync(IInterfaceLogger log)
    {
        // Request status from device.
        var status = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create("ST", "STACK"))[0];

        if (status.Length < 5) throw new InvalidOperationException($"too few response lines");

        // Map protocol reply to answer.
        var response = new BurdenStatus();

        foreach (var item in status.Skip(status.Length - 5).Take(4))
            switch (item)
            {
                case "ON:0":
                    response.Active = false;
                    break;
                case "ON:1":
                    response.Active = true;
                    break;
                default:
                    switch (item[..2])
                    {
                        case "B:":
                            response.Burden = item[2..];
                            break;
                        case "R:":
                            response.Range = item[2..];
                            break;
                        case "N:":
                            response.Step = item[2..];
                            break;
                        default:
                            throw new InvalidOperationException($"bad status information: {item}");
                    }

                    break;
            }

        return response;
    }

    /// <inheritdoc/>
    public async Task<BurdenVersion> GetVersionAsync(IInterfaceLogger log)
    {
        // Request version from device.
        var version = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create("AV", "AVACK"))[0];

        if (version.Length < 3) throw new InvalidOperationException($"too few response lines");

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

    /// <inheritdoc/>
    public Task SetBurdenAsync(string burden, IInterfaceLogger log)
        => device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"SB{burden}", "SBACK"))[0];

    /// <inheritdoc/>
    public async Task SetPermanentCalibrationAsync(string burden, string range, string step, Calibration calibration, IInterfaceLogger log)
    {
        // Get the full previous setting.
        var fullName = $"{burden};{range};{step}";
        var before = await GetCalibrationInfoAsync(burden, range, step, log) ?? throw new ArgumentException($"{fullName} can not be calibrated");

        // Update
        await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create(
                $"SA{fullName};0x{calibration.Resistive.Coarse:x2};0x{calibration.Resistive.Fine:x2};0x{calibration.Inductive.Coarse:x2};0x{calibration.Inductive.Fine:x2};{before.Item2}", "SAACK"))[0];
    }

    /// <inheritdoc/>
    public Task SetRangeAsync(string range, IInterfaceLogger log)
        => device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"SR{range}", "SRACK"))[0];

    /// <inheritdoc/>
    public Task SetStepAsync(string step, IInterfaceLogger log)
        => device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"SN{step}", "SNACK"))[0];

    /// <inheritdoc/>
    public Task SetTransientCalibrationAsync(Calibration calibration, IInterfaceLogger log)
        => device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create($"SF0x{calibration.Resistive.Coarse:x2};0x{calibration.Resistive.Fine:x2};0x{calibration.Inductive.Coarse:x2};0x{calibration.Inductive.Fine:x2}", "SFACK"))[0];
}
