
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
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
    protected readonly ISerialPortConnectionExecutor Device;

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
        _validator = validator;
        Capabilities = capabilities;
        Logger = logger;

        Device = device.CreateExecutor(InterfaceLogSourceTypes.Source);
    }

    /// <inheritdoc/>
    public abstract Task<SourceCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger);

    /// <inheritdoc/>
    public virtual TargetLoadpoint? GetCurrentLoadpoint(IInterfaceLogger interfaceLogger) => Loadpoint;

    /// <inheritdoc/>
    public virtual async Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        /* Always validate the loadpoint against the device capabilities. */
        var isValid = _validator.IsValid(loadpoint, await GetCapabilities(logger));

        if (isValid != SourceApiErrorCodes.SUCCESS)
            return isValid;

        Logger.LogTrace("Loadpoint set, source turned on.");

        /* Remember loadpoint even if we were not able to completly send it to the device. */
        Loadpoint = loadpoint;
        Info.SavedAt = DateTime.Now;

        try
        {
            await Task.WhenAll(Device.Execute(logger, Translator.ToSerialPortRequests(loadpoint)));

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
    public abstract Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger);

    /// <inheritdoc/>
    public virtual LoadpointInfo GetActiveLoadpointInfo(IInterfaceLogger interfaceLogger) => Info;

    /// <inheritdoc/>
    public abstract Task SetDosageMode(IInterfaceLogger logger, bool on);

    /// <inheritdoc/>
    public abstract Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant);

    /// <inheritdoc/>
    public abstract Task StartDosage(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task CancelDosage(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant);

    /// <inheritdoc/>
    public abstract bool GetAvailable(IInterfaceLogger interfaceLogger);

    /// <inheritdoc/>
    public abstract Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger);
}
