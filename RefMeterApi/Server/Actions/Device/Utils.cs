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
    public static MeasureOutput ConvertFromDINtoIEC(MeasureOutput measureOutput, int firstPhaseAngle)
    {
        // Not manipulating the original measureOutput object
        MeasureOutput result = DeepCopy(measureOutput);

        ConvertAngles(result, firstPhaseAngle);

        SwapPhaseOrder(result, firstPhaseAngle);

        return result;
    }

    /// <summary>
    /// Copys values of object, not its reference
    /// </summary>
    /// <typeparam name="T">Any type of objects</typeparam>
    /// <param name="self">object to clone</param>
    /// <returns>a clone of the object</returns>
    private static T DeepCopy<T>(T self) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(self))!;

    private static void ConvertAngles(MeasureOutput measureOutput, int firstPhaseAngle)
    {
        // All current phases are off
        if (firstPhaseAngle < 0)
            return;

        // reverse all angles
        foreach (var phase in measureOutput.Phases)
        {
            phase.AngleVoltage = (360 - phase.AngleVoltage) % 360;
            phase.AngleCurrent = (360 - phase.AngleCurrent) % 360;
        };

        // get the current angle of the first phase, that is on
        var angle = measureOutput.Phases[firstPhaseAngle].AngleCurrent;

        // reference angle is already 0°
        if (angle == 0)
            return;

        // first current angle must be 0°. The rest we add up
        foreach (var phase in measureOutput.Phases)
        {
            phase.AngleVoltage = (phase.AngleVoltage - angle + 360) % 360;
            phase.AngleCurrent = (phase.AngleCurrent - angle + 360) % 360;
        }
    }

    private static void SwapPhaseOrder(MeasureOutput measureOutput, int firstPhaseAngle)
    {
        if (firstPhaseAngle < 0 || firstPhaseAngle >= measureOutput.Phases.Count || measureOutput.Phases.Count < 3)
            return;

        if (measureOutput.PhaseOrder == "123")
            measureOutput.PhaseOrder = "132";
        else
            measureOutput.PhaseOrder = "123";
    }

}
