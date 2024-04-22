
using Microsoft.Extensions.Logging;
using SerialPortProxy;

using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort;

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
    protected readonly ISerialPortConnection Device;

    /// <summary>
    /// 
    /// </summary>
    protected readonly ICapabilitiesMap Capabilities;

    /// <summary>
    /// 
    /// </summary>
    protected TargetLoadpoint? Loadpoint { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    protected LoadpointInfo Info { get; private set; } = new();

    /// <summary>
    /// 
    /// </summary>
    protected ILoadpointTranslator Translator { get; private set; } = new T();

    protected ISourceCapabilityValidator _validator;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    /// <param name="capabilities"></param>
    /// <param name="validator"></param>
    protected CommonSource(ILogger<CommonSource<T>> logger, ISerialPortConnection device, ICapabilitiesMap capabilities, ISourceCapabilityValidator validator)
    {
        Capabilities = capabilities;
        Device = device;
        Logger = logger;
        _validator = validator;
    }

    /// <inheritdoc/>
    public abstract Task<SourceCapabilities> GetCapabilities();

    /// <inheritdoc/>
    public virtual TargetLoadpoint? GetCurrentLoadpoint() => Loadpoint;

    /// <inheritdoc/>
    public virtual async Task<SourceApiErrorCodes> SetLoadpoint(TargetLoadpoint loadpoint)
    {
        /* Always validate the loadpoint against the device capabilities. */
        var isValid = _validator.IsValid(loadpoint, await GetCapabilities());

        if (isValid != SourceApiErrorCodes.SUCCESS)
            return isValid;

        Logger.LogTrace("Loadpoint set, source turned on.");

        /* Remember loadpoint even if we were not able to completly send it to the device. */
        Loadpoint = loadpoint;
        Info.SavedAt = DateTime.Now;

        try
        {
            await Task.WhenAll(Device.CreateExecutor().Execute(Translator.ToSerialPortRequests(loadpoint)));

            Info.ActivatedAt = DateTime.Now;
            Info.IsActive = true;
        }
        catch (Exception e)
        {
            /* At least one request in the transaction failed, transaction was aborted and device not turned on - this would be the last request in the transaction. */
            Logger.LogWarning("Loadpoint set, but source could not be turned on: {0}", e);

            return SourceApiErrorCodes.SUCCESS_NOT_ACTIVATED;
        }

        return SourceApiErrorCodes.SUCCESS;
    }

    /// <inheritdoc/>
    public abstract Task<SourceApiErrorCodes> TurnOff();

    /// <inheritdoc/>
    public virtual LoadpointInfo GetActiveLoadpointInfo() => Info;

    /// <inheritdoc/>
    public abstract Task SetDosageMode(bool on);

    /// <inheritdoc/>
    public abstract Task SetDosageEnergy(double value, double meterConstant);

    /// <inheritdoc/>
    public abstract Task StartDosage();

    /// <inheritdoc/>
    public abstract Task CancelDosage();

    /// <inheritdoc/>
    public abstract Task<DosageProgress> GetDosageProgress(double meterConstant);

    /// <inheritdoc/>
    public abstract bool Available { get; }

    /// <inheritdoc/>
    public abstract Task<bool> CurrentSwitchedOffForDosage();
}
