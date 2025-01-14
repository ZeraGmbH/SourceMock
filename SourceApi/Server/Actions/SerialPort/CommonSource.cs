
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
    public abstract Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger);

    /// <inheritdoc/>
    public virtual Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(Loadpoint);

    /// <inheritdoc/>
    public virtual async Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        /* Always validate the loadpoint against the device capabilities. */
        var isValid = _validator.IsValid(loadpoint, await GetCapabilitiesAsync(logger));

        if (isValid != SourceApiErrorCodes.SUCCESS)
            return isValid;

        Logger.LogTrace("Loadpoint set, source turned on.");

        /* Remember loadpoint even if we were not able to completly send it to the device. */
        Loadpoint = loadpoint;
        Info.SavedAt = DateTime.Now;

        try
        {
            await Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, Translator.ToSerialPortRequests(loadpoint)));

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
    public abstract Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger);

    /// <inheritdoc/>
    public virtual Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(Info);

    /// <inheritdoc/>
    public abstract Task SetDosageModeAsync(IInterfaceLogger logger, bool on);

    /// <inheritdoc/>
    public abstract Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant);

    /// <inheritdoc/>
    public abstract Task StartDosageAsync(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task CancelDosageAsync(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant);

    /// <inheritdoc/>
    public abstract Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger);

    /// <inheritdoc/>
    public abstract Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task StartEnergyAsync(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task StopEnergyAsync(IInterfaceLogger logger);

    /// <inheritdoc/>
    public abstract Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger);
}
