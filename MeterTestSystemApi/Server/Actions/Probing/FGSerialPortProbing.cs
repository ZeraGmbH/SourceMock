using System.Text.RegularExpressions;
using MeterTestSystemApi.Services;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a FG30x connected to a serial port.
/// </summary>
public class FGSerialPortProbing(IInterfaceLogger logger) : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public bool EnableReader => true;

    /// <inheritdoc/>
    public void AdjustOptions(SerialPortOptions options) { }

    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(SerialProbe probe, ISerialPortConnection connection)
    {
        var executor = connection.CreateExecutor(InterfaceLogSourceTypes.MeterTestSystem, "probe");
        var request = SerialPortRequest.Create("TS", new Regex("^TS(.{8})(.{4})$"));

        await executor.ExecuteAsync(logger, request)[0];

        return new() { Message = $"FG Model {request.EndMatch!.Groups[1].Value.Trim()}", Succeeded = true };
    }
}