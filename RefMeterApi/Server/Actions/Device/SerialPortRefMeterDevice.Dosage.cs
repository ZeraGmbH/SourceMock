using System.Globalization;
using System.Text.RegularExpressions;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortRefMeterDevice
{
    private static readonly Regex EnergyWithUnitReg = new Regex(@"^(.{1,8});([0-4])$");

    /// <inheritdoc/>
    public Task CancelDosage() =>
        Task.WhenAll(_device.Execute(SerialPortRequest.Create("ADC2", "ADCACK")));

    /// <inheritdoc/>
    public async Task<DosageProgress> GetDosageProgress()
    {
        var commands = await Task.WhenAll(_device.Execute(
            SerialPortRequest.Create("ADS5", "ADSACK"),
            SerialPortRequest.Create("ADV4", "ADVACK"),
            SerialPortRequest.Create("ADV5", "ADVACK")
        ));

        var totalWithUnit = EnergyWithUnitReg.Match(commands[0][0]);
        var unit = int.Parse(totalWithUnit.Groups[2].Value, CultureInfo.InvariantCulture);

        return new DosageProgress
        {
            Progress = double.Parse(commands[2][0], CultureInfo.InvariantCulture),
            Remaining = double.Parse(commands[1][0], CultureInfo.InvariantCulture),
            Total = double.Parse(totalWithUnit.Groups[1].Value, CultureInfo.InvariantCulture),
            Unit =
                unit == 0 ? EnergyUnits.MicroWattHours :
                unit == 1 ? EnergyUnits.MilliWattHours :
                unit == 2 ? EnergyUnits.WattHours :
                unit == 3 ? EnergyUnits.KiloWattHours :
                EnergyUnits.MegaWattHours
        };
    }

    /// <inheritdoc/>
    public Task SetDosageEnergy(double value, EnergyUnits unit)
    {
        /* Coarse pre check for a maximum of 8 characters. */
        if (value < 0 || value >= 1E8)
            throw new ArgumentOutOfRangeException(nameof(value));

        /* Convert to string. */
        var valueAsString = value.ToString(CultureInfo.InvariantCulture);

        /* If there is a dot at the very first position prepend a zero. */
        var dot = valueAsString.IndexOf('.');

        if (dot == 0)
        {
            valueAsString = $"0{valueAsString}";

            dot = 1;
        }

        /* If the string has more than 8 characters we have to clip it. */
        if (valueAsString.Length > 8)
        {
            /* Unable to clip without corrupting the value. */
            if (dot < 0 || dot > 8)
                throw new ArgumentOutOfRangeException(nameof(value));

            /* This may lead to a slight rounding error. */
            valueAsString = valueAsString.Substring(0, dot == 7 ? 7 : 8);
        }

        /* Translate the unit to a string. */
        var unitAsString =
            unit == EnergyUnits.MicroWattHours ? 0 :
            unit == EnergyUnits.MilliWattHours ? 1 :
            unit == EnergyUnits.WattHours ? 2 :
            unit == EnergyUnits.KiloWattHours ? 3 :
            unit == EnergyUnits.MegaWattHours ? 4 :
            throw new ArgumentException(nameof(unit));

        return _device.Execute(SerialPortRequest.Create($"ADP{valueAsString};{unitAsString}", "ADPACK"))[0];
    }

    /// <inheritdoc/>
    public Task SetDosageMode(bool on)
    {
        var onAsNumber = on ? 3 : 4;

        return Task.WhenAll(_device.Execute(SerialPortRequest.Create($"ADC{onAsNumber}", "ADCACK")));
    }

    /// <inheritdoc/>
    public Task StartDosage() =>
        Task.WhenAll(_device.Execute(SerialPortRequest.Create("ADC1", "ADCACK")));
}
