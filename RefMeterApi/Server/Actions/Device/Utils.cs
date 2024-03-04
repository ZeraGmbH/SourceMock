using Newtonsoft.Json;
using RefMeterApi.Models;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Helper functions
/// </summary>
public static class Utils
{
    /// <summary>
    /// Converts MeasureOutput given by the device in DIN form, to IEC, that is supported by the api
    /// </summary>
    /// <param name="measureOutput"></param>
    /// <param name="firstPhaseAngle"></param>
    /// <returns></returns>
    public static MeasuredLoadpoint ConvertFromDINtoIEC(MeasuredLoadpoint measureOutput, int firstPhaseAngle)
    {
        // Not manipulating the original measureOutput object
        MeasuredLoadpoint result = DeepCopy(measureOutput);

        ConvertAngles(result, firstPhaseAngle);

        return result;
    }

    /// <summary>
    /// Copys values of object, not its reference
    /// </summary>
    /// <typeparam name="T">Any type of objects</typeparam>
    /// <param name="self">object to clone</param>
    /// <returns>a clone of the object</returns>
    private static T DeepCopy<T>(T self) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(self))!;

    private static void ConvertAngles(MeasuredLoadpoint measureOutput, int firstPhaseAngle)
    {
        // reverse all angles
        foreach (var phase in measureOutput.Phases)
        {
            phase.Voltage.AcComponent.Angle = (360 - phase.Voltage.AcComponent.Angle) % 360;
            phase.Current.AcComponent.Angle = (360 - phase.Current.AcComponent.Angle) % 360;
        };

        // All current phases are off
        if (firstPhaseAngle < 0)
            return;

        // get the current angle of the first phase, that is on
        var angle = measureOutput.Phases[firstPhaseAngle].Current.AcComponent.Angle;

        // reference angle is already 0°
        if (angle == 0)
            return;

        // first current angle must be 0°. The rest we add up
        foreach (var phase in measureOutput.Phases)
        {
            phase.Voltage.AcComponent.Angle = (phase.Voltage.AcComponent.Angle - angle + 360) % 360;
            phase.Current.AcComponent.Angle = (phase.Current.AcComponent.Angle - angle + 360) % 360;
        }
    }
}
