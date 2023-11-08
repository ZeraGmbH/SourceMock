using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RefMeterApi.Models;
using SerialPortProxy;
using ZstdSharp.Unsafe;

namespace RefMeterApi.Actions.Device;

partial class SerialPortRefMeterDevice
{
    /// <inheritdoc/>
    public Task CancelDosage() =>
        Task.WhenAll(_device.Execute(SerialPortRequest.Create("S3CM2", "SOK3CM2")));

    /// <inheritdoc/>
    public async Task<DosageProgress> GetDosageProgress()
    {
        var active = SerialPortRequest.Create("S3SA1", new Regex(@"^SOK3SA1;([0123])$"));
        var countdown = SerialPortRequest.Create("S3MA4", new Regex(@"^SOK3MA4;(.+)$"));
        var progress = SerialPortRequest.Create("S3MA5", new Regex(@"^SOK3MA5;(.+)$"));

        await Task.WhenAll(_device.Execute(active, countdown, progress));

        return new()
        {
            Active = active.EndMatch!.Groups[1].Value == "2",
            Progress = (long)double.Parse(progress.EndMatch!.Groups[1].Value, CultureInfo.InvariantCulture),
            Remaining = (long)double.Parse(countdown.EndMatch!.Groups[1].Value, CultureInfo.InvariantCulture),
        };
    }

    /// <summary>
    /// Use the current status values from the device to calculate the 
    /// meter constant.
    /// </summary>
    /// <returns>The current meter constant.</returns>
    /// <exception cref="InvalidOperationException">Status is incomplete.</exception>
    /// <exception cref="ArgumentException">Measuring mode not supported.</exception>
    private async Task<double> GetCurrentMeterConstant()
    {
        var reply = await _device.Execute(SerialPortRequest.Create("AST", "ASTACK"))[0];

        double? voltage = null, current = null;
        string? mode = null;

        foreach (var value in reply)
            if (value.StartsWith("UB="))
                voltage = double.Parse(value.Substring(3), CultureInfo.InvariantCulture);
            else if (value.StartsWith("IB="))
                current = double.Parse(value.Substring(3), CultureInfo.InvariantCulture);
            else if (value.StartsWith("M="))
                mode = value.Substring(2);

        if (!voltage.HasValue || !current.HasValue || string.IsNullOrEmpty(mode))
            throw new InvalidOperationException("AST status incomplete");

        var phases =
            mode[0] == '4' ? 3d :
            mode[0] == '3' ? 2d :
            mode[0] == '2' ? 1d :
            throw new ArgumentException($"unsupported measurement mode {mode}");

        return 1000d * 3600d * 60000d / (phases * (double)voltage * (double)current);
    }

    /// <inheritdoc/>
    public async Task SetDosageEnergy(double value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        var meterConst = await GetCurrentMeterConstant();
        var impulses = (long)Math.Round(meterConst * value);

        await Task.WhenAll(_device.Execute(SerialPortRequest.Create($"S3PS46;{impulses:0000000000}", "SOK3PS46")));
    }

    /// <inheritdoc/>
    public Task SetDosageMode(bool on)
    {
        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(_device.Execute(SerialPortRequest.Create($"S3CM{onAsNumber}", $"SOK3CM{onAsNumber}")));
    }

    /// <inheritdoc/>
    public Task StartDosage() =>
        Task.WhenAll(_device.Execute(SerialPortRequest.Create("S3CM1", "SOK3CM1")));
}
