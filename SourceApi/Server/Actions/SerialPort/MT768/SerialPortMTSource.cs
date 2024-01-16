using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using SerialPortProxy;

using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.MT768;

/// <summary>
/// 
/// </summary>
public interface ISerialPortMTSource : ISource
{
}

/// <summary>
/// A ISource implenmentation to access a (potentially mocked) device. This
/// should be a singleton because it manages the loadpoint.
/// </summary>
public partial class SerialPortMTSource : CommonSource<MTLoadpointTranslator>, ISerialPortMTSource
{
    /// <inheritdoc/>
    public override bool Available => true;

    /// <summary>
    /// Initialize a new source implementation.
    /// </summary>
    /// <param name="logger">Logger to use.</param>
    /// <param name="device">Access to the serial port.</param>
    /// <param name="capabilities">Static capabilities lookup table.</param>
    public SerialPortMTSource(ILogger<SerialPortMTSource> logger, ISerialPortConnection device, ICapabilitiesMap capabilities) : base(logger, device, capabilities)
    {
    }

    /// <inheritdoc/>
    public override Task<SourceCapabilities> GetCapabilities()
    {
        /* Currently we assume MT768, future versions may read the firmware from the device. */
        return Task.FromResult(Capabilities.GetCapabilitiesByModel("MT786"));
    }

    /// <inheritdoc/>
    public override async Task<SourceApiErrorCodes> TurnOff()
    {
        Logger.LogTrace("Switching anything off.");

        await Task.WhenAll(Device.Execute(SerialPortRequest.Create("SUIAAAAAAAAA", "SOKUI")));

        return SourceApiErrorCodes.SUCCESS;
    }

    /// <inheritdoc/>
    public override Task<double[]> GetVoltageRanges()
    {
        return Task.FromResult(Available ? Capabilities.GetVoltageRangesByModel("MT786") : null!);
    }

    /// <inheritdoc/>
    public override Task<double[]> GetCurrentRanges()
    {
        return Task.FromResult(Available ? Capabilities.GetCurrentRangesByModel("MT786") : null!);
    }
}
