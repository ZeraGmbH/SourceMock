using MeterTestSystemApi.Actions.Probing.TcpIp;
using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.Models.Logging;
using ZeraDevices.ErrorCalculator.STM;

namespace MeterTestSystemApi.Actions.Probing.KoaLa;

/// <summary>
/// 
/// </summary>
public class STMv1IPProbe([FromKeyedServices(ErrorCalculatorConnectionTypes.TCP)] IMadConnection connection, IInterfaceLogger logger) : IIPProbeExecutor
{
    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(IPProbe probe)
    {
        try
        {
            await connection.InitializeAsync("Probe", probe.EndPoint.ToString(), 2000, 2000);

            var version = await Mad1ErrorCalculator.GetFirmwareVersionAsync(connection, logger);

            return new() { Succeeded = true, Message = $"{version.ModelName} {version.Version}" };
        }
        catch (Exception e)
        {
            return new() { Succeeded = false, Message = e.Message };
        }
    }
}