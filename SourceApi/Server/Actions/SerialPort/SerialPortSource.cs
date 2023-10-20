using SerialPortProxy;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort;


/// <summary>
/// 
/// </summary>
public class SerialPortSource : ISource
{
    private readonly ILogger<SerialPortSource> _logger;

    private readonly SerialPortService _device;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    public SerialPortSource(ILogger<SerialPortSource> logger, SerialPortService device)
    {
        _device = device;
        _logger = logger;
    }

    private Loadpoint? _loadpoint;

    /// <inheritdoc/>
    public SourceCapabilities GetCapabilities()
    {
        return CapabilitiesMap.GetCapabilitiesByModel("MT786");
    }

    /// <inheritdoc/>
    public Loadpoint? GetCurrentLoadpoint() => _loadpoint;

    /// <inheritdoc/>
    public SourceResult SetLoadpoint(Loadpoint loadpoint)
    {
        var isValid = SourceCapabilityValidator.IsValid(loadpoint, GetCapabilities());

        if (isValid != SourceResult.SUCCESS)
            return isValid;

        _logger.LogTrace("Loadpoint set, source turned on.");

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
