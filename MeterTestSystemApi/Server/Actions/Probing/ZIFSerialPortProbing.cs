using MeterTestSystemApi.Services;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;
using ZIFApi.Actions;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a ZIF socket connected to a serial port.
/// </summary>
public class ZIFSerialPortProbing(IInterfaceLogger logger) : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public bool EnableReader => false;

    /// <inheritdoc/>
    public void AdjustOptions(SerialPortOptions options) { }

    private readonly int[] _VersionRequest = [0xc2];

    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(SerialProbe probe, ISerialPortConnection connection)
    {
        var executor = connection.CreateExecutor(InterfaceLogSourceTypes.ZIF, "probe");

        var reply = await executor.RawExecuteAsync(
            logger,
            (port, logger) =>
            {
                /* Convert to protocol and send. */
                port.RawWrite(PowerMaster8121.CommandToProtocol(_VersionRequest));

                /* Wait for the reply. */
                return PowerMaster8121.ReadResponse(_VersionRequest, port, []);
            }
        );

        if (reply.Length != 5) return new() { Succeeded = false, Message = "invalid reply" };

        return new() { Message = $"PowerMaster8121 ZIF Version {BitConverter.ToInt32(reply)}.{reply[4]}", Succeeded = true };
    }
}