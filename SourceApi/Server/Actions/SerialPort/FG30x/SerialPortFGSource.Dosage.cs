using System.Text.RegularExpressions;
using SerialPortProxy;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.FG30x;

partial class SerialPortFGSource
{
    /// <inheritdoc/>
    public override Task CancelDosage()
    {
        TestConfigured();

        return Task.WhenAll(Device.Execute(SerialPortRequest.Create("3CM2", "OK3CM2")));
    }

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgress()
    {
        TestConfigured();

        /* Get all actual values - unit is pulse interval. */
        var activeReq = SerialPortRequest.Create("3SA1", new Regex(@"^OK3SA1;([0123])$"));
        var countdownReq = SerialPortRequest.Create("3MA1", new Regex(@"^OK3MA1;(.+)$"));
        var totalReq = SerialPortRequest.Create("3PA45", new Regex(@"^OK3PA45;(.+)$"));

        await Task.WhenAll(Device.Execute(activeReq, countdownReq, totalReq));

        /* Scale actual values to energy - in Wh. */

        double remaining = double.Parse(countdownReq.EndMatch!.Groups[1].Value) * 1000d;
        double total = double.Parse(totalReq.EndMatch!.Groups[1].Value) * 1000d;

        return new()
        {
            Active = activeReq.EndMatch!.Groups[1].Value == "2",
            Progress = total - remaining,
            Remaining = remaining,
            Total = total,
        };
    }

    /// <inheritdoc/>
    public override Task SetDosageEnergy(double value)
    {
        TestConfigured();

        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        return Device.Execute(SerialPortRequest.Create($"3PS45;{value / 1000d}", "OK3PS45"))[0];
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(bool on)
    {
        TestConfigured();

        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(Device.Execute(SerialPortRequest.Create($"3CM{onAsNumber}", $"OK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override Task StartDosage()
    {
        TestConfigured();

        return Task.WhenAll(Device.Execute(SerialPortRequest.Create("3CM1", "OK3CM1")));
    }

    /// <inheritdoc/>
    public override Task<bool> CurrentSwitchedOffForDosage()
    {
        throw new NotImplementedException();
    }
}
