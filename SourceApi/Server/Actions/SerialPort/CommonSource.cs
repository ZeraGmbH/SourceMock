
using SerialPortProxy;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CommonSource<T> : ISource where T : ILoadpointTranslator, new()
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly ILogger<CommonSource<T>> Logger;

    /// <summary>
    /// 
    /// </summary>
    protected readonly SerialPortConnection Device;

    /// <summary>
    /// 
    /// </summary>
    protected Loadpoint? Loadpoint { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    protected LoadpointInfo Info { get; private set; } = new();

    /// <summary>
    /// 
    /// </summary>
    protected ILoadpointTranslator Translator { get; private set; } = new T();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    protected CommonSource(ILogger<CommonSource<T>> logger, SerialPortConnection device)
    {
        Device = device;
        Logger = logger;
    }

    /// <inheritdoc/>
    public abstract Task<SourceCapabilities> GetCapabilities();

    /// <inheritdoc/>
    public virtual Loadpoint? GetCurrentLoadpoint() => Loadpoint;

    /// <inheritdoc/>
    public virtual async Task<SourceResult> SetLoadpoint(Loadpoint loadpoint)
    {
        /* Always validate the loadpoint against the device capabilities. */
        var isValid = SourceCapabilityValidator.IsValid(loadpoint, await GetCapabilities());

        if (isValid != SourceResult.SUCCESS)
            return isValid;

        Logger.LogTrace("Loadpoint set, source turned on.");

        /* Remember loadpoint even if we were not able to completly send it to the device. */
        Loadpoint = loadpoint;
        Info.SavedAt = DateTime.Now;

        try
        {
            await Task.WhenAll(Device.Execute(Translator.ToSerialPortRequests(loadpoint)));

            Info.ActivatedAt = DateTime.Now;
        }
        catch (Exception e)
        {
            /* At least one request in the transaction failed, transaction was aborted and device not turned on - this would be the last request in the transaction. */
            Logger.LogWarning("Loadpoint set, but source could not be turned on: {0}", e);

            return SourceResult.SUCCESS_NOT_ACTIVATED;
        }

        return SourceResult.SUCCESS;
    }

    /// <inheritdoc/>
    public abstract Task<SourceResult> TurnOff();

    /// <inheritdoc/>
    public abstract Task<DeviceFirmwareVersion> GetFirmwareVersion();

    /// <inheritdoc/>
    public virtual LoadpointInfo GetActiveLoadpointInfo() => Info;

    /// <inheritdoc/>
    public abstract Task SetDosageMode(bool on);

    /// <inheritdoc/>
    public abstract Task SetDosageEnergy(double value);

    /// <inheritdoc/>
    public abstract Task StartDosage();

    /// <inheritdoc/>
    public abstract Task CancelDosage();

    /// <inheritdoc/>
    public abstract Task<DosageProgress> GetDosageProgress();
}
