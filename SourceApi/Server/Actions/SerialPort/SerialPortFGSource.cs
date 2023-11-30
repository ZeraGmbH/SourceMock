using SerialPortProxy;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
public class SerialPortFGSource : ISource
{
    private readonly ILogger<SerialPortFGSource> _logger;

    private readonly SerialPortConnection _device;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    public SerialPortFGSource(ILogger<SerialPortFGSource> logger, SerialPortConnection device)
    {
        _device = device;
        _logger = logger;
    }


    /// <inheritdoc/>
    public Task CancelDosage()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Loadpoint? GetCurrentLoadpoint()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgress()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetDosageEnergy(double value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetDosageMode(bool on)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task StartDosage()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceResult> TurnOff()
    {
        throw new NotImplementedException();
    }
}
