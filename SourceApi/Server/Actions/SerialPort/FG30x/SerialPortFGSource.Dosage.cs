using System.Text.RegularExpressions;
using SerialPortProxy;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.FG30x;

partial class SerialPortFGSource
{
    /// <inheritdoc/>
    public override Task CancelDosage() =>
        Task.WhenAll(Device.Execute(SerialPortRequest.Create("3CM2", "OK3CM2")));

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgress()
    {
        /* Get all actual values - unit is pulse interval. */
        var active = SerialPortRequest.Create("3SA1", new Regex(@"^OK3SA1;([0123])$"));
        var countdown = SerialPortRequest.Create("3MA1", new Regex(@"^OK3MA1;(.+)$"));
        var progress = SerialPortRequest.Create("3SA4", new Regex(@"^OK3SA4;(.+)$"));
        var total = SerialPortRequest.Create("3PA45", new Regex(@"^OK3PA45;(.+)$"));

        await Task.WhenAll(Device.Execute(active, countdown, progress, total));

        /* Scale actual values to energy - in Wh. */
        return new()
        {
            Active = active.EndMatch!.Groups[1].Value == "2",
            Progress = double.Parse(progress.EndMatch!.Groups[1].Value) * 1000d,
            Remaining = double.Parse(countdown.EndMatch!.Groups[1].Value) * 1000d,
            Total = double.Parse(total.EndMatch!.Groups[1].Value) * 1000d,
        };
    }

    /// <inheritdoc/>
    public override Task SetDosageEnergy(double value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        return Device.Execute(SerialPortRequest.Create($"3PS45;{value / 1000d}", "OK3PS45"))[0];
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(bool on)
    {
        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(Device.Execute(SerialPortRequest.Create($"3CM{onAsNumber}", $"OK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override Task StartDosage() =>
        Task.WhenAll(Device.Execute(SerialPortRequest.Create("3CM1", "OK3CM1")));
}
