using System.Text.RegularExpressions;

using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.MT768;

partial class SerialPortMTSource
{
    /// <inheritdoc/>
    public override Task CancelDosageAsync(IInterfaceLogger logger) =>
        Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create("S3CM2", "SOK3CM2")));

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        /* Get all actual values - unit is pulse interval. */
        var active = SerialPortRequest.Create("S3SA1", new Regex(@"^SOK3SA1;([0123])$"));
        var countdown = SerialPortRequest.Create("S3MA4", new Regex(@"^SOK3MA4;(.+)$"));
        var progress = SerialPortRequest.Create("S3MA5", new Regex(@"^SOK3MA5;(.+)$"));
        var total = SerialPortRequest.Create("S3SA5", new Regex(@"^SOK3SA5;(.+)$"));

        await Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, active, countdown, progress, total));

        /* Scale actual values to energy - in Wh. */
        return new()
        {
            Active = active.EndMatch!.Groups[1].Value == "2",
            Progress = new Impulses(double.Parse(progress.EndMatch!.Groups[1].Value)) / meterConstant,
            Remaining = new Impulses(double.Parse(countdown.EndMatch!.Groups[1].Value)) / meterConstant,
            Total = new Impulses(double.Parse(total.EndMatch!.Groups[1].Value)) / meterConstant,
        };
    }

    /// <inheritdoc/>
    public override async Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, ActiveEnergy.Zero, nameof(value));

        /* Calculate the number of impulses from the energy (in Wh) and the meter constant. */
        var impulses = value * meterConstant;

        await Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create($"S3PS46;{impulses.Format("0000000000")}", "SOK3PS46")));
    }

    /// <inheritdoc/>
    public override Task SetDosageModeAsync(IInterfaceLogger logger, bool on)
    {
        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create($"S3CM{onAsNumber}", $"SOK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override Task StartDosageAsync(IInterfaceLogger logger) =>
        Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create("S3CM1", "SOK3CM1")));

    /// <inheritdoc/>
    public override async Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger)
    {
        /* Ask device. */
        var dosage = SerialPortRequest.Create("S3SA1", new Regex(@"^SOK3SA1;([0123])$"));
        var mode = SerialPortRequest.Create("S3SA3", new Regex(@"^SOK3SA3;([012])$"));

        await Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, dosage, mode));

        /* Current should be switched off if dosage mode is on mode dosage itself is not yet active. */
        return mode.EndMatch?.Groups[1].Value == "2" && dosage.EndMatch?.Groups[1].Value == "1";
    }

    /// <inheritdoc/>
    public override Task StartEnergyAsync(IInterfaceLogger logger)
       => Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create("AET1", "AETACK")));


    /// <inheritdoc/>
    public override Task StopEnergyAsync(IInterfaceLogger logger)
       => Task.WhenAll(Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create("AET0", "AETACK")));

    /// <inheritdoc/>
    public override async Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger)
    {
        var reply = await Device.ExecuteAsync(logger, CancellationToken.None, SerialPortRequest.Create("AEV", "AEVACK"))[0];

        if (reply.Length < 2)
            throw new InvalidOperationException($"wrong number of response lines - expected 2 but got {reply.Length}");

        return new(double.Parse(reply[^2]));
    }
}
