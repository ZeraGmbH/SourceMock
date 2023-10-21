using SerialPortProxy;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort;


/// <summary>
/// A ISource implenmentation to access a (potentially mocked) device. This
/// should be a singleton because it manages the loadpoint.
/// </summary>
public class SerialPortSource : ISource
{
    private readonly ILogger<SerialPortSource> _logger;

    private readonly SerialPortConnection _device;

    /// <summary>
    /// Initialize a new source implementation.
    /// </summary>
    /// <param name="logger">Logger to use.</param>
    /// <param name="device">Access to the serial port.</param>
    public SerialPortSource(ILogger<SerialPortSource> logger, SerialPortConnection device)
    {
        _device = device;
        _logger = logger;
    }

    private Loadpoint? _loadpoint;

    /// <inheritdoc/>
    public SourceCapabilities GetCapabilities()
    {
        /* Currently we assume MT768, future versions may read the firmware from the device. */
        return CapabilitiesMap.GetCapabilitiesByModel("MT786");
    }

    /// <inheritdoc/>
    public Loadpoint? GetCurrentLoadpoint() => _loadpoint;

    /// <inheritdoc/>
    public SourceResult SetLoadpoint(Loadpoint loadpoint)
    {
        /* Always validate the loadpoint against the device capabilities. */
        var isValid = SourceCapabilityValidator.IsValid(loadpoint, GetCapabilities());

        if (isValid != SourceResult.SUCCESS)
            return isValid;

        _logger.LogTrace("Loadpoint set, source turned on.");

        /* Remember loadpoint even if we were not able to completly send it to the device. */
        _loadpoint = loadpoint;

        /* TODO: SetLoadpoint should return a Task and communication should use await to return Thread to ThreadPool while waiting */

        Task.WhenAll(_device.Execute(LoadpointTranslator.ToSerialPortRequests(loadpoint))).Wait();

        return SourceResult.SUCCESS;
    }

    /// <inheritdoc/>
    public SourceResult TurnOff()
    {
        throw new NotImplementedException();
    }
}
