using System.Text.RegularExpressions;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.Dosage;

namespace SourceApi.Actions.SerialPort.FG30x;

partial class SerialPortFGSource
{
    /// <inheritdoc/>
    public override async Task CancelDosageAsync(IInterfaceLogger logger)
    {
        await TestConfiguredAsync(logger);

        await Task.WhenAll(Device.ExecuteAsync(logger, SerialPortRequest.Create("3CM2", "OK3CM2")));
    }

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        await TestConfiguredAsync(logger);

        /* Get all actual values - unit is pulse interval. */
        var activeReq = SerialPortRequest.Create("3SA1", new Regex(@"^OK3SA1;([0123])$"));
        var countdownReq = SerialPortRequest.Create("3MA1", new Regex(@"^OK3MA1;(.+)$"));
        var totalReq = SerialPortRequest.Create("3PA45", new Regex(@"^OK3PA45;(.+)$"));

        await Task.WhenAll(Device.ExecuteAsync(logger, activeReq, countdownReq, totalReq));

        /* Scale actual values to energy - in Wh. */
        var remaining = new ActiveEnergy(double.Parse(countdownReq.EndMatch!.Groups[1].Value) * 1000d);
        var total = new ActiveEnergy(double.Parse(totalReq.EndMatch!.Groups[1].Value) * 1000d);

        return new()
        {
            Active = activeReq.EndMatch!.Groups[1].Value == "2",
            Progress = total - remaining,
            Remaining = remaining,
            Total = total,
        };
    }

    /// <inheritdoc/>
    public override async Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        await TestConfiguredAsync(logger);

        if (value < ActiveEnergy.Zero) throw new ArgumentOutOfRangeException(nameof(value));

        await Device.ExecuteAsync(logger, SerialPortRequest.Create($"3PS45;{(double)value / 1000d}", "OK3PS45"))[0];
    }

    /// <inheritdoc/>
    public override async Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
    {
        await TestConfiguredAsync(logger);

        var onAsNumber = on ? 3 : 4;

        await Task.WhenAll(Device.ExecuteAsync(logger, SerialPortRequest.Create($"3CM{onAsNumber}", $"OK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override async Task StartDosageAsync(IInterfaceLogger logger)
    {
        await TestConfiguredAsync(logger);

        await Task.WhenAll(Device.ExecuteAsync(logger, SerialPortRequest.Create("3CM1", "OK3CM1")));
    }

    /// <inheritdoc/>
    public override async Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
    {
        /* Ask device. */
        var dosage = SerialPortRequest.Create("3SA1", new Regex(@"^OK3SA1;([0123])$"));
        var mode = SerialPortRequest.Create("3SA3", new Regex(@"^OK3SA3;([012])$"));

        await Task.WhenAll(Device.ExecuteAsync(logger, dosage, mode));

        /* Current should be switched off if dosage mode is on mode dosage itself is not yet active. */
        return mode.EndMatch?.Groups[1].Value == "2" && dosage.EndMatch?.Groups[1].Value == "1";
    }

    /// <inheritdoc/>
    public override Task StartEnergyAsync(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task StopEnergyAsync(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger) => throw new NotImplementedException();

}
