using System.Text.RegularExpressions;

using SerialPortProxy;

using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.MT768;

partial class SerialPortMTSource
{
    /// <inheritdoc/>
    public override Task CancelDosage() =>
        Task.WhenAll(Device.CreateExecutor().Execute(SerialPortRequest.Create("S3CM2", "SOK3CM2")));

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgress(double meterConstant)
    {
        /* Request the current meter constant. */
        meterConstant /= 1000d;

        /* Get all actual values - unit is pulse interval. */
        var active = SerialPortRequest.Create("S3SA1", new Regex(@"^SOK3SA1;([0123])$"));
        var countdown = SerialPortRequest.Create("S3MA4", new Regex(@"^SOK3MA4;(.+)$"));
        var progress = SerialPortRequest.Create("S3MA5", new Regex(@"^SOK3MA5;(.+)$"));
        var total = SerialPortRequest.Create("S3SA5", new Regex(@"^SOK3SA5;(.+)$"));

        await Task.WhenAll(Device.CreateExecutor().Execute(active, countdown, progress, total));

        /* Scale actual values to energy - in Wh. */
        return new()
        {
            Active = active.EndMatch!.Groups[1].Value == "2",
            Progress = double.Parse(progress.EndMatch!.Groups[1].Value) / meterConstant,
            Remaining = double.Parse(countdown.EndMatch!.Groups[1].Value) / meterConstant,
            Total = double.Parse(total.EndMatch!.Groups[1].Value) / meterConstant,
        };
    }

    /// <inheritdoc/>
    public override async Task SetDosageEnergy(double value, double meterConstant)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        /* Calculate the number of impulses from the energy (in Wh) and the meter constant. */
        var impulses = (long)Math.Round(meterConstant * value / 1000d);

        await Task.WhenAll(Device.CreateExecutor().Execute(SerialPortRequest.Create($"S3PS46;{impulses:0000000000}", "SOK3PS46")));
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(bool on)
    {
        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(Device.CreateExecutor().Execute(SerialPortRequest.Create($"S3CM{onAsNumber}", $"SOK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override Task StartDosage() =>
        Task.WhenAll(Device.CreateExecutor().Execute(SerialPortRequest.Create("S3CM1", "SOK3CM1")));

    /// <inheritdoc/>
    public override async Task<bool> CurrentSwitchedOffForDosage()
    {
        /* Ask device. */
        var dosage = SerialPortRequest.Create("S3SA1", new Regex(@"^SOK3SA1;([0123])$"));
        var mode = SerialPortRequest.Create("S3SA3", new Regex(@"^SOK3SA3;([012])$"));

        await Task.WhenAll(Device.CreateExecutor().Execute(dosage, mode));

        /* Current should be switched off if dosage mode is on mode dosage itself is not yet active. */
        return mode.EndMatch?.Groups[1].Value == "2" && dosage.EndMatch?.Groups[1].Value == "1";
    }
}
