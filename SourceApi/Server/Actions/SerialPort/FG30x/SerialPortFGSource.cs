using Microsoft.Extensions.Logging;
using SerialPortProxy;

using SourceApi.Actions.Source;
using SourceApi.Exceptions;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public interface ISerialPortFGSource : ISource
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltage"></param>
    /// <param name="current"></param>
    /// <param name="voltageAux"></param>
    /// <param name="currentAux"></param>
    /// <returns></returns>
    void SetAmplifiers(VoltageAmplifiers voltage, CurrentAmplifiers current, VoltageAuxiliaries voltageAux, CurrentAuxiliaries currentAux);
}

/// <summary>
/// 
/// </summary>
public partial class SerialPortFGSource : CommonSource<FGLoadpointTranslator>, ISerialPortFGSource
{
    private VoltageAmplifiers? VoltageAmplifier;

    private CurrentAmplifiers? CurrentAmplifier;

    private VoltageAuxiliaries? VoltageAuxiliary;

    private CurrentAuxiliaries? CurrentAuxiliary;


    /// <inheritdoc/>
    public override bool Available =>
        VoltageAmplifier.HasValue &&
        CurrentAmplifier.HasValue &&
        VoltageAuxiliary.HasValue &&
        CurrentAuxiliary.HasValue;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="device"></param>
    /// <param name="capabilities"></param>
    /// <param name="validator"></param>
    public SerialPortFGSource(ILogger<SerialPortFGSource> logger, ISerialPortConnection device, ICapabilitiesMap capabilities, ISourceCapabilityValidator validator) : base(logger, device, capabilities, validator)
    {
    }

    private void TestConfigured()
    {
        if (!Available) throw new SourceNotReadyException();
    }

    /// <inheritdoc/>
    public override Task<SourceCapabilities> GetCapabilities()
    {
        TestConfigured();

        return Task.FromResult(Available ? Capabilities.GetCapabilitiesByAmplifiers(VoltageAmplifier!.Value, CurrentAmplifier!.Value) : null!);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltage"></param>
    /// <param name="current"></param>
    /// <param name="voltageAux"></param>
    /// <param name="current8"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetAmplifiers(VoltageAmplifiers voltage, CurrentAmplifiers current, VoltageAuxiliaries voltageAux, CurrentAuxiliaries current8)
    {
        if (Available) throw new InvalidOperationException("Source already initialized");

        CurrentAmplifier = current;
        VoltageAmplifier = voltage;
        VoltageAuxiliary = voltageAux;
        CurrentAuxiliary = current8;
    }

    /// <inheritdoc/>
    public override async Task<SourceApiErrorCodes> TurnOff()
    {
        TestConfigured();

        Logger.LogTrace("Switching anything off.");

        await Task.WhenAll(Device.CreateExecutor().Execute(SerialPortRequest.Create("UIAAAAAAAAA", "OKUI")));

        Info.IsActive = false;

        return SourceApiErrorCodes.SUCCESS;
    }
}
