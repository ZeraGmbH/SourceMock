using System.Globalization;
using System.Text.RegularExpressions;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortRefMeterDevice
{
    private static readonly Regex MatchErrorStatus1 = new(@"^([0-1])([0-3])$");

    private static readonly Regex MatchErrorStatus2 = new(@"^[-]?[^.]*\.[^.]+$");

    private static readonly Regex MatchErrorStatus3 = new(@"^([^;]+);([^;]+)$");

    /// <inheritdoc/>
    public Task AbortErrorMeasurement() =>
        _device.Execute(SerialPortRequest.Create("AEE", "AEEACK"))[0];

    /// <inheritdoc/>
    public async Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        var replies = await _device.Execute(SerialPortRequest.Create("AES1", "AESACK"))[0];

        var result = new ErrorMeasurementStatus { State = ErrorMeasurementStates.NotActive };

        ErrorMeasurementStates? state = null;

        var gotError = false;

        foreach (var reply in replies)
            if (state == null)
            {
                /* Expect a status information. */
                var match = MatchErrorStatus1.Match(reply);

                if (!match.Success)
                    continue;

                switch (match.Groups[2].Value)
                {
                    case "0":
                        state = result.State = ErrorMeasurementStates.NotActive;
                        break;
                    case "1":
                        state = result.State = ErrorMeasurementStates.Active;
                        break;
                    case "2":
                        state = result.State = ErrorMeasurementStates.Running;
                        break;
                    case "3":
                        state = result.State = ErrorMeasurementStates.Finished;
                        break;
                }
            }
            else if (!gotError)
            {
                /* Expect an error information. */
                var match = MatchErrorStatus2.Match(reply);

                if (!match.Success)
                    continue;

                gotError = true;

                if (reply == "oo.oo" || reply == "--.--")
                    continue;

                try
                {
                    /* Check for a fairly regular value. */
                    var error = double.Parse(reply);

                    if (double.IsNaN(error) || double.IsInfinity(error))
                        gotError = false;
                    else
                        result.ErrorValue = error;
                }
                catch (System.Exception)
                {
                    gotError = false;
                }
            }
            else
            {
                /* Expect progress and enery information. */
                var match = MatchErrorStatus3.Match(reply);

                if (!match.Success)
                    continue;

                /* Try parse numbers. */
                try
                {
                    var progress = double.Parse(match.Groups[1].Value);
                    var energy = double.Parse(match.Groups[2].Value);

                    if (double.IsNaN(progress) || double.IsInfinity(progress))
                        continue;

                    if (double.IsNaN(energy) || double.IsInfinity(energy))
                        continue;


                    result.Progress = progress;
                    result.Energy = energy;

                    return result;
                }
                catch (Exception)
                {
                    continue;
                }
            }

        return result;
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        var (rawMeter, powMeter) = ClipNumberToProtocol((long)Math.Round(meterConstant * 1E5), 16);

        if (powMeter > 10)
            throw new ArgumentOutOfRangeException(nameof(impulses));

        var (rawImpulses, powImpulses) = ClipNumberToProtocol(impulses, 11);

        if (powImpulses > 5)
            throw new ArgumentOutOfRangeException(nameof(impulses));

        return _device.Execute(SerialPortRequest.Create($"AEP{rawMeter};{powMeter:00};{rawImpulses};{powImpulses}", "AEPACK"))[0];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Tuple<string, int> ClipNumberToProtocol(long number, int limit)
    {
        if (number <= 0)
            throw new ArgumentOutOfRangeException(nameof(number));

        /* Total precision is at most 6 digits scaled with up to 100,000. */
        var asString = number.ToString("".PadLeft(limit, '0')).TrimStart('0');

        if (asString.Length > limit)
            throw new ArgumentOutOfRangeException(nameof(number));

        /* Check for the scale factor. */
        var power = asString.Length - 6;

        if (power <= 0)
            return Tuple.Create(asString, 0);

        /* Must eventually round. */
        var scale = Math.Pow(10d, power);

        number = (long)(Math.Round(number / scale) * scale);

        /* Reconstruct the string. */
        asString = number.ToString("".PadLeft(limit, '0')).TrimStart('0');

        if (asString.Length > limit)
            throw new ArgumentOutOfRangeException(nameof(number));

        /* Recalculate the power factor. */
        power = Math.Max(0, asString.Length - 6);

        /* Clip off scaling. */
        return Tuple.Create(asString[..^power], power);
    }

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous) =>
        _device.Execute(SerialPortRequest.Create(continuous ? "AEB1" : "AEB0", "AEBACK"))[0];
}
