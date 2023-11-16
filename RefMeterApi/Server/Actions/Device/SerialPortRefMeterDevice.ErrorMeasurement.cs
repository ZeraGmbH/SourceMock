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
                        state = result.State = ErrorMeasurementStates.Run;
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
                    var error = double.Parse(reply, CultureInfo.CurrentUICulture);

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
                    var progress = double.Parse(match.Groups[1].Value, CultureInfo.CurrentUICulture);
                    var energy = double.Parse(match.Groups[2].Value, CultureInfo.CurrentUICulture);

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
        if (meterConstant <= 0)
            throw new ArgumentOutOfRangeException(nameof(meterConstant), "Meter constant must not positive.");

        if (meterConstant > 99999900000)
            throw new ArgumentOutOfRangeException(nameof(meterConstant), "Meter constant must be at most 99999900000.");

        if (impulses <= 0)
            throw new ArgumentOutOfRangeException(nameof(impulses), "Number of impulses must be positive.");

        if (impulses > 99999900000)
            throw new ArgumentOutOfRangeException(nameof(impulses), "Number of impulses must be at most 99999900000.");

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous) =>
        _device.Execute(SerialPortRequest.Create(continuous ? "AEB1" : "AEB1", "AEBACK"))[0];
}
