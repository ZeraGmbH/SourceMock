using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Exceptions;
using SourceApi.Model;
using ZERA.WebSam.Shared.Models.Source;

namespace SourceApi.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public interface ISerialPortFGSource : ISource
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="interfaceLogger"></param>
    /// <param name="voltage"></param>
    /// <param name="current"></param>
    /// <param name="voltageAux"></param>
    /// <param name="currentAux"></param>
    /// <returns></returns>
    Task SetAmplifiersAsync(IInterfaceLogger interfaceLogger, VoltageAmplifiers voltage, CurrentAmplifiers current, VoltageAuxiliaries voltageAux, CurrentAuxiliaries currentAux);
}

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="device"></param>
/// <param name="capabilities"></param>
/// <param name="validator"></param>
public partial class SerialPortFGSource(ILogger<SerialPortFGSource> logger, [FromKeyedServices("MeterTestSystem")] ISerialPortConnection device, ICapabilitiesMap capabilities, ISourceCapabilityValidator validator) : CommonSource<FGLoadpointTranslator>(logger, device, capabilities, validator), ISerialPortFGSource
{
    private VoltageAmplifiers? VoltageAmplifier;

    private CurrentAmplifiers? CurrentAmplifier;

    private VoltageAuxiliaries? VoltageAuxiliary;

    private CurrentAuxiliaries? CurrentAuxiliary;


    /// <inheritdoc/>
    public override Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger)
        => Task.FromResult(VoltageAmplifier.HasValue && CurrentAmplifier.HasValue && VoltageAuxiliary.HasValue && CurrentAuxiliary.HasValue);

    private async Task TestConfiguredAsync(IInterfaceLogger interfaceLogger)
    {
        if (!await GetAvailableAsync(interfaceLogger)) throw new SourceNotReadyException();
    }

    /// <inheritdoc/>
    public override async Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger)
    {
        await TestConfiguredAsync(interfaceLogger);

        return await GetAvailableAsync(interfaceLogger) ? Capabilities.GetCapabilitiesByAmplifiers(VoltageAmplifier!.Value, CurrentAmplifier!.Value) : null!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interfaceLogger"></param>
    /// <param name="voltage"></param>
    /// <param name="current"></param>
    /// <param name="voltageAux"></param>
    /// <param name="current8"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SetAmplifiersAsync(IInterfaceLogger interfaceLogger, VoltageAmplifiers voltage, CurrentAmplifiers current, VoltageAuxiliaries voltageAux, CurrentAuxiliaries current8)
    {
        if (await GetAvailableAsync(interfaceLogger)) throw new InvalidOperationException("Source already initialized");

        CurrentAmplifier = current;
        VoltageAmplifier = voltage;
        VoltageAuxiliary = voltageAux;
        CurrentAuxiliary = current8;
    }

    /// <inheritdoc/>
    public override async Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger)
    {
        await TestConfiguredAsync(logger);

        Logger.LogTrace("Switching anything off.");

        await Task.WhenAll(Device.ExecuteAsync(logger, SerialPortRequest.Create("UIAAAAAAAAA", "OKUI")));

        Info.IsActive = false;

        return SourceApiErrorCodes.SUCCESS;
    }
}
