using MeterTestSystemApi.Services;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;
using ZIFApi.Actions;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a ZIF socket connected to a serial port.
/// </summary>
public class ZIFSerialPortProbing(IInterfaceLogger logger, ILogger<ZIFSerialPortProbing> _logger) : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public bool EnableReader => false;

    /// <inheritdoc/>
    public void AdjustOptions(SerialPortOptions options) { }

    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(ISerialPortConnection connection)
    {
        var reply = await PowerMaster8121.Execute(connection, "probe", logger, _logger, raw => raw, 0xc2);

        if (reply.Length != 5) return new() { Succeeded = false, Message = "invalid reply" };

        return new() { Message = $"PowerMaster8121 ZIF Version {BitConverter.ToInt32(reply)}.{reply[4]}", Succeeded = true };
    }
}