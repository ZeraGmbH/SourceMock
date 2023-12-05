using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SerialPortProxy;

using SourceApi.Actions.Source;
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
public class SerialPortFGSource : CommonSource<FGLoadpointTranslator>, ISerialPortFGSource
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
    public SerialPortFGSource(ILogger<SerialPortFGSource> logger, ISerialPortConnection device, ICapabilitiesMap capabilities) : base(logger, device, capabilities)
    {
    }

    /// <inheritdoc/>
    public override Task CancelDosage()
    {
        // 290 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<SourceCapabilities> GetCapabilities()
    {
        return Task.FromResult(Available ? Capabilities.GetCapabilitiesByAmplifiers(VoltageAmplifier!.Value, CurrentAmplifier!.Value) : null!);
    }

    /// <inheritdoc/>
    public override Task<DosageProgress> GetDosageProgress()
    {
        // 243/252 or 3SA/3MA
        throw new NotImplementedException();
    }

    public void SetAmplifiers(VoltageAmplifiers voltage, CurrentAmplifiers current, VoltageAuxiliaries voltageAux, CurrentAuxiliaries current8)
    {
        if (Available) throw new InvalidOperationException("Source already initialized");

        CurrentAmplifier = current;
        VoltageAmplifier = voltage;
        VoltageAuxiliary = voltageAux;
        CurrentAuxiliary = current8;
    }

    /// <inheritdoc/>
    public override Task SetDosageEnergy(double value)
    {
        // 210/211 or 3PS
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(bool on)
    {
        // 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task StartDosage()
    {
        // 242 or 3CM
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<SourceResult> TurnOff()
    {
        // 2S0/2S1
        throw new NotImplementedException();
    }
}
