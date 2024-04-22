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
    /// <param name="validator">Validate loadpoint against device capabilities.</param>
    public SerialPortMTSource(ILogger<SerialPortMTSource> logger, ISerialPortConnection device, ICapabilitiesMap capabilities, ISourceCapabilityValidator validator) : base(logger, device, capabilities, validator)
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

        await Task.WhenAll(Device.CreateExecutor().Execute(SerialPortRequest.Create("SUIAAAAAAAAA", "SOKUI")));

        Info.IsActive = false;

        return SourceApiErrorCodes.SUCCESS;
    }
}
