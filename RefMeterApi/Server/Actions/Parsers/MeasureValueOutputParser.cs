using System.Globalization;
using System.Text.RegularExpressions;
using RefMeterApi.Models;

namespace RefMeterApi.Actions.Parsers;

/// <summary>
/// 
/// </summary>
public static class MeasureValueOutputParser
{
    private static readonly Regex _valueReg = new Regex("^(\\d{1,3});(.+)$");

    /// <summary>
    /// Create measure output structure from AME reply sequence.
    /// </summary>
    /// <param name="replies">String data as received from an AME request.</param>
    /// <returns>All data as provided by the device.</returns>
    /// <exception cref="ArgumentException">Parameter is not a valid AME reply sequence.</exception>
    public static MeasureOutput Parse(string[] replies)
    {
        /* Make sure this is an AME reply sequence. */
        if (replies.Length < 1 || replies[^1] != "AMEACK")
            throw new ArgumentException("missing AMEACK", nameof(replies));

        /* Prepare response with three phases. */
        var response = new MeasureOutput
        {
            Phases = { new MeasureOutputPhase(), new MeasureOutputPhase(), new MeasureOutputPhase(), }
        };

        for (var i = 0; i < replies.Length - 1; i++)
        {
            /* Chck for a value with index. */
            var reply = _valueReg.Match(replies[i]);

            if (!reply.Success)
                throw new ArgumentException($"bad reply {replies[i]}", nameof(replies));

            /* Decode index and value. */
            var index = int.Parse(reply.Groups[1].Value, CultureInfo.InvariantCulture);
            var value = double.Parse(reply.Groups[2].Value, CultureInfo.InvariantCulture);

            /* Copy value to the appropriate field. */
            switch (index)
            {
                case 0:
                    response.Phases[0].Voltage = value;
                    break;
                case 1:
                    response.Phases[1].Voltage = value;
                    break;
                case 2:
                    response.Phases[2].Voltage = value;
                    break;
                case 3:
                    response.Phases[0].Current = value;
                    break;
                case 4:
                    response.Phases[1].Current = value;
                    break;
                case 5:
                    response.Phases[2].Current = value;
                    break;
                case 6:
                    response.Phases[0].AngleVoltage = value;
                    break;
                case 7:
                    response.Phases[1].AngleVoltage = value;
                    break;
                case 8:
                    response.Phases[2].AngleVoltage = value;
                    break;
                case 9:
                    response.Phases[0].AngleCurrent = value;
                    break;
                case 10:
                    response.Phases[1].AngleCurrent = value;
                    break;
                case 11:
                    response.Phases[2].AngleCurrent = value;
                    break;
                case 12:
                    response.Phases[0].Angle = value;
                    break;
                case 13:
                    response.Phases[1].Angle = value;
                    break;
                case 14:
                    response.Phases[2].Angle = value;
                    break;
                case 15:
                    response.Phases[0].ActivePower = value;
                    break;
                case 16:
                    response.Phases[1].ActivePower = value;
                    break;
                case 17:
                    response.Phases[2].ActivePower = value;
                    break;
                case 18:
                    response.Phases[0].ReactivePower = value;
                    break;
                case 19:
                    response.Phases[1].ReactivePower = value;
                    break;
                case 20:
                    response.Phases[2].ReactivePower = value;
                    break;
                case 21:
                    response.Phases[0].ApparentPower = value;
                    break;
                case 22:
                    response.Phases[1].ApparentPower = value;
                    break;
                case 23:
                    response.Phases[2].ApparentPower = value;
                    break;
                case 24:
                    response.ActivePower = value;
                    break;
                case 25:
                    response.ReactivePower = value;
                    break;
                case 26:
                    response.ApparentPower = value;
                    break;
                case 27:
                    response.PhaseOrder = (int)value;
                    break;
                case 28:
                    response.Frequency = value;
                    break;
            }
        }

        return response;
    }
}
