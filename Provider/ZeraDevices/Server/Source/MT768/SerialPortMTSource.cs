using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Provider;
using ZERA.WebSam.Shared.Provider.Exceptions;

namespace ZeraDevices.Source.MT768;

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
/// <param name="logger">Logger to use.</param>
/// <param name="device">Access to the serial port.</param>
/// <param name="capabilities">Static capabilities lookup table.</param>
/// <param name="validator">Validate loadpoint against device capabilities.</param>
public partial class SerialPortMTSource(ILogger<SerialPortMTSource> logger, [FromKeyedServices("ZERASerial")] ISerialPortConnection device, ICapabilitiesMap capabilities, ISourceCapabilityValidator validator) : CommonSource<MTLoadpointTranslator>(logger, device, capabilities, validator), ISerialPortMTSource
{
    /// <inheritdoc/>
    public override Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(true);

    /// <inheritdoc/>
    public override Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger)
    {
        /* Currently we assume MT768, future versions may read the firmware from the device. */
        return Task.FromResult(Capabilities.GetCapabilitiesByModel("MT786"));
    }

    /// <inheritdoc/>
    public override async Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger)
    {
        Logger.LogTrace("Switching anything off.");

        await Task.WhenAll(Device.ExecuteAsync(logger, SerialPortRequest.Create("SUIAAAAAAAAA", "SOKUI")));

        Info.IsActive = false;

        return SourceApiErrorCodes.SUCCESS;
    }
}
