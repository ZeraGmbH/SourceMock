using System.Text.RegularExpressions;

using SerialPortProxy;

using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.MT768;

partial class SerialPortMTSource
{
    /// <inheritdoc/>
    public override Task CancelDosage() =>
        Task.WhenAll(Device.Execute(SerialPortRequest.Create("S3CM2", "SOK3CM2")));

    /// <inheritdoc/>
    public override async Task<DosageProgress> GetDosageProgress()
    {
        /* Request the current meter constant. */
        var measureConstant = await GetCurrentMeterConstant() / 1000d;

        /* Get all actual values - unit is pulse interval. */
        var active = SerialPortRequest.Create("S3SA1", new Regex(@"^SOK3SA1;([0123])$"));
        var countdown = SerialPortRequest.Create("S3MA4", new Regex(@"^SOK3MA4;(.+)$"));
        var progress = SerialPortRequest.Create("S3MA5", new Regex(@"^SOK3MA5;(.+)$"));
        var total = SerialPortRequest.Create("S3SA5", new Regex(@"^SOK3SA5;(.+)$"));

        await Task.WhenAll(Device.Execute(active, countdown, progress, total));

        /* Scale actual values to energy - in Wh. */
        return new()
        {
            Active = active.EndMatch!.Groups[1].Value == "2",
            Progress = double.Parse(progress.EndMatch!.Groups[1].Value) / measureConstant,
            Remaining = double.Parse(countdown.EndMatch!.Groups[1].Value) / measureConstant,
            Total = double.Parse(total.EndMatch!.Groups[1].Value) / measureConstant,
        };
    }

    /// <summary>
    /// Use the current status values from the device to calculate the 
    /// meter constant.
    /// </summary>
    /// <returns>The current meter constant (impulses per kWh).</returns>
    /// <exception cref="InvalidOperationException">Status is incomplete.</exception>
    /// <exception cref="ArgumentException">Measuring mode not supported.</exception>
    private async Task<double> GetCurrentMeterConstant()
    {
        var reply = await Device.Execute(SerialPortRequest.Create("AST", "ASTACK"))[0];

        /* We need the range of voltage and current and the measurement mode as well. */
        double? voltage = null, current = null;
        string? mode = null;

        foreach (var value in reply)
            if (value.StartsWith("UB="))
                voltage = double.Parse(value.Substring(3));
            else if (value.StartsWith("IB="))
                current = double.Parse(value.Substring(3));
            else if (value.StartsWith("M="))
                mode = value.Substring(2);

        if (!voltage.HasValue || !current.HasValue || string.IsNullOrEmpty(mode))
            throw new InvalidOperationException("AST status incomplete");

        var phases =
            mode[0] == '4' ? 3d :
            mode[0] == '3' ? 2d :
            mode[0] == '2' ? 1d :
            throw new ArgumentException($"unsupported measurement mode {mode}");

        /* Calculate according to formula - see MT78x_MAN_EXT_GB.pdf section 5.6.*/
        return 1000d * 3600d * 60000d / (phases * (double)voltage * (double)current);
    }

    /// <inheritdoc/>
    public override async Task SetDosageEnergy(double value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        /* Calculate the number of impulses from the energy (in Wh) and the meter constant. */
        var meterConst = await GetCurrentMeterConstant();
        var impulses = (long)Math.Round(meterConst * value / 1000d);

        await Task.WhenAll(Device.Execute(SerialPortRequest.Create($"S3PS46;{impulses:0000000000}", "SOK3PS46")));
    }

    /// <inheritdoc/>
    public override Task SetDosageMode(bool on)
    {
        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(Device.Execute(SerialPortRequest.Create($"S3CM{onAsNumber}", $"SOK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public override Task StartDosage() =>
        Task.WhenAll(Device.Execute(SerialPortRequest.Create("S3CM1", "SOK3CM1")));

    /// <inheritdoc/>
    public override Task<bool> CurrentSwitchedOffForDosage()
    {
        throw new NotImplementedException();
    }
}
