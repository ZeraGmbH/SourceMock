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
        // 290 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public LoadpointInfo GetActiveLoadpointInfo()
    {
        // Software only
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceCapabilities> GetCapabilities()
    {
        // Software only
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Loadpoint? GetCurrentLoadpoint()
    {
        // Software only
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<DosageProgress> GetDosageProgress()
    {
        // 243/252 or 3SA/3MA
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        // 2TS
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetDosageEnergy(double value)
    {
        // 210/211 or 3PS
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetDosageMode(bool on)
    {
        // 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        // FR, IP, UP, UI
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task StartDosage()
    {
        // 242 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SourceResult> TurnOff()
    {
        // 2S0/2S1
        throw new NotImplementedException();
    }
}
