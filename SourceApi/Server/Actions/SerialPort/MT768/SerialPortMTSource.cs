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
    public override async Task<double[]> GetVoltageRanges()
    {
        Logger.LogTrace("Requesting voltage ranges.");

        var replies = await Device.Execute(SerialPortRequest.Create("AVI", "AVIACK"))[0];
        var result = new List<double>();

        foreach (var reply in replies)
            try
            {
                if (reply != "AVIACK" && double.TryParse(reply, out var bound))
                    result.Add(bound);
            }
            catch (Exception)
            {
                /* Just ignore anything not looking like a number. */
            }

        return result.Order().ToArray();
    }

    /// <inheritdoc/>
    public override async Task<double[]> GetCurrentRanges()
    {
        Logger.LogTrace("Requesting voltage ranges.");

        var replies = await Device.Execute(SerialPortRequest.Create("ACI", "ACIACK"))[0];
        var result = new List<double>();

        foreach (var reply in replies)
            try
            {
                if (reply != "ACIACK" && double.TryParse(reply, out var bound))
                    result.Add(bound);
            }
            catch (Exception)
            {
                /* Just ignore anything not looking like a number. */
            }

        return result.Order().ToArray();
    }
}
