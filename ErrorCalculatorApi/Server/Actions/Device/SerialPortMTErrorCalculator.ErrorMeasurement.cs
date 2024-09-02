using System.Text.RegularExpressions;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device;

partial class SerialPortMTErrorCalculator
{
    /// <summary>
    /// Pattern for the first status line - current status of the error measurement.
    /// </summary>
    private static readonly Regex MatchErrorStatus1 = new(@"^([0-1])([0-3])$");

    /// <summary>
    /// Pattern for the second status line - current error result or empty indicator.
    /// </summary>
    private static readonly Regex MatchErrorStatus2 = new(@"^[-]?[^.]*\.[^.]+$");

    /// <summary>
    /// Pattern for the third status line - progress and energy.
    /// </summary>
    private static readonly Regex MatchErrorStatus3 = new(@"^([^;]+);([^;]+)$");

    /// <inheritdoc/>
    public Task AbortErrorMeasurementAsync(IInterfaceLogger logger) => _device.Execute(logger, SerialPortRequest.Create("AEE", "AEEACK"))[0];

    /// <inheritdoc/>
    public async Task<ErrorMeasurementStatus> GetErrorStatusAsync(IInterfaceLogger logger)
    {
        /* Ask the device for the current status. */
        var replies = await _device.Execute(logger, SerialPortRequest.Create("AES1", "AESACK"))[0];

        /* Prepare for the result. */
        var result = new ErrorMeasurementStatus { State = ErrorMeasurementStates.NotActive };

        /* Analyse the status - skip all unknown noise on the serial line. */
        ErrorMeasurementStates? state = null;

        var gotError = false;

        foreach (var reply in replies)
            if (state == null)
            {
                /* Expect a status information imn the first line. */
                var match = MatchErrorStatus1.Match(reply);

                if (!match.Success)
                {
                    _logger.LogWarning("Got unrecognized status line {Reply} while waiting for status.", reply);

                    continue;
                }

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
                /* Expect an error information in the second line. */
                var match = MatchErrorStatus2.Match(reply);

                if (!match.Success)
                {
                    _logger.LogWarning("Got unrecognized status line {Reply} while waiting for result.", reply);

                    continue;
                }

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
                catch (Exception e)
                {
                    _logger.LogWarning("Unable to parse error from {Reply}: {Exception}", reply, e.Message);

                    gotError = false;
                }
            }
            else
            {
                /* Expect progress and enery information in the third line. */
                var match = MatchErrorStatus3.Match(reply);

                if (!match.Success)
                {
                    _logger.LogWarning("Got unrecognized status line {Reply} while waiting for progress.", reply);

                    continue;
                }

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

                    return result;
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Unable to parse progress from {Reply}: {Exception}", reply, e.Message);

                    continue;
                }
            }

        return result;
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParametersAsync(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant)
    {
        /* Create the text representation of the meter constant and see if it fits the protocol requirements. */
        var (rawMeter, powMeter) = ClipNumberToProtocol((long)Math.Round((double)dutMeterConstant * 1E5), 16);

        if (powMeter > 10)
        {
            _logger.LogDebug("Invalid meter constant {MeterConstant}", dutMeterConstant);

            throw new ArgumentOutOfRangeException(nameof(dutMeterConstant));
        }

        /* Create the text representation of the number of impulses and see if it fits the protocol requirements. */
        var (rawImpulses, powImpulses) = ClipNumberToProtocol((long)impulses, 11);

        if (powImpulses > 5)
        {
            _logger.LogDebug("Invalid number of impluses {Impulses}", impulses);

            throw new ArgumentOutOfRangeException(nameof(impulses));
        }

        /* Now we can send the resulting texts and power factors to the device. */
        return _device.Execute(logger, SerialPortRequest.Create($"AEP{rawMeter};{powMeter:00};{rawImpulses};{powImpulses}", "AEPACK"))[0];
    }

    /// <summary>
    /// Try to find the closest text representation of a number which fits the
    /// protocol needes.
    /// </summary>
    /// <param name="number">The number to convert to a string.</param>
    /// <param name="limit">The maximum number of digits allowed.</param>
    /// <returns>Text representation of the number and a scaling factor as a power of 10.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Number can not be converted to an appropriate string.</exception>
    public Tuple<string, int> ClipNumberToProtocol(long number, int limit)
    {
        if (number <= 0)
        {
            _logger.LogWarning("Number {Number} must be positive", number);

            throw new ArgumentOutOfRangeException(nameof(number));
        }

        /* Make the number a string and see if fits the limit of digits. */
        var asString = number.ToString("".PadLeft(limit, '0')).TrimStart('0');

        if (asString.Length > limit)
        {
            _logger.LogDebug("Number {Number} too large", number);

            throw new ArgumentOutOfRangeException(nameof(number));
        }

        /* Check for the scale factor to make the resulting string fit in 6 digits. */
        var power = asString.Length - 6;

        if (power <= 0)
            return Tuple.Create(asString, 0);

        /* Must eventually round. */
        var scale = Math.Pow(10d, power);

        number = (long)(Math.Round(number / scale) * scale);

        /* Reconstruct the string and test again - may be a corner case due to roundings. */
        asString = number.ToString("".PadLeft(limit, '0')).TrimStart('0');

        if (asString.Length > limit)
        {
            _logger.LogDebug("Number {Number} too large", number);

            throw new ArgumentOutOfRangeException(nameof(number));
        }

        /* Recalculate the power factor. */
        power = Math.Max(0, asString.Length - 6);

        /* Clip off scaling and report the final text representation and the power factor. */
        return Tuple.Create(asString[..^power], power);
    }

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnectionsAsync(IInterfaceLogger logger) => Task.FromResult<ErrorCalculatorMeterConnections[]>([]);

    /// <inheritdoc/>
    public Task StartErrorMeasurementAsync(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection) =>
        _device.Execute(logger, SerialPortRequest.Create(continuous ? "AEB1" : "AEB0", "AEBACK"))[0];

    /// <inheritdoc/>
    public Task<Impulses?> GetNumberOfDeviceUnderTestImpulsesAsync(IInterfaceLogger logger) => Task.FromResult<Impulses?>(null);
}
