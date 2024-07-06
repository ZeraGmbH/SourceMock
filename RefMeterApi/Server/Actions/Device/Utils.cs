using RefMeterApi.Models;
using SharedLibrary;
using SharedLibrary.DomainSpecific;

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
        var result = LibUtils.DeepCopy(measureOutput);

        ConvertAngles(result, firstPhaseAngle);

        return result;
    }

    private static void ConvertAngles(MeasuredLoadpoint measureOutput, int firstPhaseAngle)
    {
        // reverse all angles
        foreach (var phase in measureOutput.Phases)
        {
            phase.Voltage.AcComponent!.Angle = (-phase.Voltage.AcComponent!.Angle).Normalize();
            phase.Current.AcComponent!.Angle = (-phase.Current.AcComponent!.Angle).Normalize();
        };

        // All current phases are off
        if (firstPhaseAngle < 0)
            return;

        // get the current angle of the first phase, that is on
        var angle = measureOutput.Phases[firstPhaseAngle].Current.AcComponent!.Angle;

        // reference angle is already 0°
        if ((double)angle == 0)
            return;

        // first current angle must be 0°. The rest we add up
        foreach (var phase in measureOutput.Phases)
        {
            phase.Voltage.AcComponent!.Angle = (phase.Voltage.AcComponent!.Angle - angle).Normalize();
            phase.Current.AcComponent!.Angle = (phase.Current.AcComponent!.Angle - angle).Normalize();
        }
    }
}
