using System.Text.RegularExpressions;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.FG30x;

partial class SerialPortFGSource
{
    /// <inheritdoc/>
    public override Task CancelDosage(IInterfaceLogger logger)
    {
        TestConfigured(logger);

        return Task.WhenAll(Device.Execute(logger, SerialPortRequest.Create("3CM2", "OK3CM2")));
    }

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        TestConfigured(logger);

        /* Get all actual values - unit is pulse interval. */
        var activeReq = SerialPortRequest.Create("3SA1", new Regex(@"^OK3SA1;([0123])$"));
        var countdownReq = SerialPortRequest.Create("3MA1", new Regex(@"^OK3MA1;(.+)$"));
        var totalReq = SerialPortRequest.Create("3PA45", new Regex(@"^OK3PA45;(.+)$"));

        await Task.WhenAll(Device.Execute(logger, activeReq, countdownReq, totalReq));

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
    public override Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        TestConfigured(logger);

        if (value < ActiveEnergy.Zero)
            throw new ArgumentOutOfRangeException(nameof(value));

        return Device.Execute(logger, SerialPortRequest.Create($"3PS45;{(double)value / 1000d}", "OK3PS45"))[0];
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(IInterfaceLogger logger, bool on)
    {
        TestConfigured(logger);

        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(Device.Execute(logger, SerialPortRequest.Create($"3CM{onAsNumber}", $"OK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override Task StartDosage(IInterfaceLogger logger)
    {
        TestConfigured(logger);

        return Task.WhenAll(Device.Execute(logger, SerialPortRequest.Create("3CM1", "OK3CM1")));
    }

    /// <inheritdoc/>
    public override async Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger)
    {
        /* Ask device. */
        var dosage = SerialPortRequest.Create("3SA1", new Regex(@"^OK3SA1;([0123])$"));
        var mode = SerialPortRequest.Create("3SA3", new Regex(@"^OK3SA3;([012])$"));

        await Task.WhenAll(Device.Execute(logger, dosage, mode));

        /* Current should be switched off if dosage mode is on mode dosage itself is not yet active. */
        return mode.EndMatch?.Groups[1].Value == "2" && dosage.EndMatch?.Groups[1].Value == "1";
    }
}
