using System.Text.RegularExpressions;
using MeterTestSystemApi.Services;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a MT786/MT707/MT3000 connected to a serial port.
/// </summary>
public class MTSerialPortProbing(IInterfaceLogger logger) : ISerialPortProbeExecutor
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <inheritdoc/>
    public bool EnableReader => true;

    /// <inheritdoc/>
    public void AdjustOptions(SerialPortOptions options) { }

    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(ISerialPortConnection connection)
    {
        var executor = connection.CreateExecutor(InterfaceLogSourceTypes.MeterTestSystem, "probe");
        var reply = await executor.ExecuteAsync(logger, SerialPortRequest.Create("AAV", "AAVACK"))[0];

        if (reply.Length < 2) return new() { Succeeded = false, Message = "invalid reply" };

        var versionMatch = _versionReg.Match(reply[^2]);

        if (versionMatch?.Success != true) return new() { Succeeded = false, Message = $"bad version {reply[0]}" };

        return new() { Message = $"MT Model {versionMatch.Groups[1].Value}", Succeeded = true };
    }
}