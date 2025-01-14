using System.Text.RegularExpressions;
using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions.Device;

/// <summary>
/// Tag interface to help using the mock in simulation scenarios.
/// </summary>
public interface IBurdenMock : IBurden
{
    /// <summary>
    /// Check if the source is simulated.
    /// </summary>
    bool HasMockedSource { get; }
}

/// <summary>
/// Implementation of the burden communication interface.
/// </summary>
/// <param name="connection">Serial port connection to the hardware.</param>
public class Burden([FromKeyedServices("Burden")] ISerialPortConnection connection) : IBurdenMock, ISerialPortOwner
{
    private static readonly Regex ValueReg = new(@"^(\d{1,3});(.+)$");

    private static readonly Regex ExtraReg = new(@"^(.*)\.0+$");

    // Port access helper.
    private readonly ISerialPortConnectionExecutor device = connection?.CreateExecutor(InterfaceLogSourceTypes.Burden)!;

    /// <inheritdoc/>
    public bool IsAvailable { get; } = connection != null;

    /// <inheritdoc/>
    public bool HasMockedSource => connection is ISerialPortConnectionMock mockedConnection && mockedConnection.Port is IBurdenSerialPortMock;

    /// <inheritdoc/>
    public async Task<string[]> GetBurdensAsync(IInterfaceLogger log)
    {
        // Request burdens from device.
        var burdens = await device.ExecuteAsync(log, SerialPortRequest.Create("AB", "ABACK"))[0];

        if (burdens.Length < 1) throw new InvalidOperationException($"too few response lines");

        return [.. burdens.Take(burdens.Length - 1)];
    }

    private async Task<Tuple<Calibration, string>?> GetCalibrationInfoAsync(string burden, string range, string step, IInterfaceLogger log)
    {
        // Request values from device.
        var values = await device.ExecuteAsync(log, SerialPortRequest.Create($"GA{burden};{range};{step}", "GAACK"))[0];

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
        var status = await device.ExecuteAsync(log, SerialPortRequest.Create("ST", "STACK"))[0];

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
        var version = await device.ExecuteAsync(log, SerialPortRequest.Create("AV", "AVACK"))[0];

        if (version.Length < 3) throw new InvalidOperationException($"too few response lines");

        // Create reply.
        return new() { Version = version[^3], Supplement = version[^2] };
    }

    /// <inheritdoc/>
    public Task ProgramAsync(string? burden, IInterfaceLogger log)
        // Enforce a read timeout of 5 Minutes - in worst case serial line will be blocked for that duration.
        => device.ExecuteAsync(log, SerialPortRequest.Create($"PR{burden ?? string.Empty}", "PRACK", 300000))[0];

    /// <inheritdoc/>
    public Task SetActiveAsync(bool on, IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create($"ON{(on ? 1 : 0)}", "ONACK"))[0];

    /// <inheritdoc/>
    public Task SetBurdenAsync(string burden, IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create($"SB{burden}", "SBACK"))[0];

    /// <inheritdoc/>
    public async Task SetPermanentCalibrationAsync(string burden, string range, string step, Calibration calibration, IInterfaceLogger log)
    {
        // Get the full previous setting.
        var fullName = $"{burden};{range};{step}";
        var before = await GetCalibrationInfoAsync(burden, range, step, log) ?? throw new ArgumentException($"{fullName} can not be calibrated");

        // Clip extra data
        var match = ExtraReg.Match(before.Item2);
        var extra = match.Success ? $"{match.Groups[1].Value}.0" : before.Item2;

        // Update
        await device.ExecuteAsync(log, SerialPortRequest.Create(
            $"SA{fullName};0x{calibration.Resistive.Coarse:x2};0x{calibration.Resistive.Fine:x2};0x{calibration.Inductive.Coarse:x2};0x{calibration.Inductive.Fine:x2};{extra}", "SAACK"))[0];
    }

    /// <inheritdoc/>
    public Task SetRangeAsync(string range, IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create($"SR{range}", "SRACK"))[0];

    /// <inheritdoc/>
    public Task SetStepAsync(string step, IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create($"SN{step}", "SNACK"))[0];

    /// <inheritdoc/>
    public Task SetTransientCalibrationAsync(Calibration calibration, IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create($"SF0x{calibration.Resistive.Coarse:x2};0x{calibration.Resistive.Fine:x2};0x{calibration.Inductive.Coarse:x2};0x{calibration.Inductive.Fine:x2}", "SFACK"))[0];

    /// <inheritdoc/>
    public async Task<BurdenValues> MeasureAsync(IInterfaceLogger log)
    {
        /* Execute the request and get the answer from the device. */
        var replies = await device.ExecuteAsync(log, SerialPortRequest.Create("ME", "MEACK"))[0];

        /* Prepare response with a single phase. */
        var response = new BurdenValues();

        for (var i = 0; i < replies.Length - 1; i++)
        {
            /* Chck for a value with index. */
            var reply = replies[i];
            var match = ValueReg.Match(reply);

            if (!match.Success) continue;

            /* Decode index and value - make sure that parsing is not messed by local operating system regional settings. */
            int index;
            double value;

            try
            {
                index = int.Parse(match.Groups[1].Value);
                value = double.Parse(match.Groups[2].Value);
            }
            catch (FormatException)
            {
                continue;
            }

            /* Copy value to the appropriate field. */
            switch (index)
            {
                case 0:
                    response.Voltage = new(value);
                    break;
                case 3:
                    response.Current = new(value);
                    break;
                case 9:
                    response.Angle = new(value);
                    break;
                case 12:
                    response.PowerFactor = new(value);
                    break;
                case 21:
                    response.ApparentPower = new(value);
                    break;
            }
        }

        return response;
    }

    /// <inheritdoc/>
    public Task CancelCalibrationAsync(IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create("CC", "CCACK"))[0];

    /// <inheritdoc/>
    public Task SetMeasuringCalibrationAsync(bool on, IInterfaceLogger log)
        => device.ExecuteAsync(log, SerialPortRequest.Create($"MR{(on ? 1 : 0)}", "MRACK"))[0];

    /// <inheritdoc/>
    public async Task<string[]> GetRangesAsync(string burden, IInterfaceLogger log)
    {
        var ranges = await device.ExecuteAsync(log, SerialPortRequest.Create($"AR{burden}", "ARACK"))[0];

        return [.. ranges.Take(ranges.Length - 1)];
    }

    /// <inheritdoc/>
    public async Task<string[]> GetStepsAsync(string burden, IInterfaceLogger log)
    {
        var steps = await device.ExecuteAsync(log, SerialPortRequest.Create($"AN{burden}", "ANACK"))[0];

        return [.. steps.Take(steps.Length - 1)];
    }

    /// <inheritdoc/>
    public async Task<SerialPortReply[]> ExecuteAsync(IEnumerable<SerialPortCommand> requests, IInterfaceLogger logger)
    {
        var commands =
            requests
                .Select(r =>
                    r.UseRegularExpression
                        ? SerialPortRequest.Create(r.Command, new Regex(r.Reply))
                        : SerialPortRequest.Create(r.Command, r.Reply))
                .ToArray();

        await Task.WhenAll(device.ExecuteAsync(logger, commands));

        return [.. commands.Select(SerialPortReply.FromRequest)];
    }
}
