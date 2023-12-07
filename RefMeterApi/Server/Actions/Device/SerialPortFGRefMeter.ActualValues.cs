using System.Text.RegularExpressions;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortFGRefMeter
{
    private static readonly Regex AuReply = new(@"^AUR(.{5})S(.{5})T(.{5})$");

    private static readonly Regex AiReply = new(@"^AIR(.{5})S(.{5})T(.{5})$");

    private static readonly Regex MpReply = new(@"^MPR([^;]+);S([^;]+);T([^;]+)$");

    private static readonly Regex MqReply = new(@"^MQR([^;]+);S([^;]+);T([^;]+)$");

    private static readonly Regex MsReply = new(@"^MSR([^;]+);S([^;]+);T([^;]+)$");

    private static readonly Regex AfReply = new(@"^AF(.+)$");

    private static readonly Regex AwReply = new(@"^AWR(.{5})(.{5})S(.{5})(.{5})T(.{5})(.{5})$");

    private static readonly Regex BuReply = new(@"^BU(.+)$");

    private static readonly Regex BiReply = new(@"^BI(.+)$");

    private static Tuple<double, double, double> GetAbsolutePhaseValues(string reply, Regex pattern)
    {
        var match = pattern.Match(reply);

        return Tuple.Create(
            double.Parse(match.Groups[1].Value),
            double.Parse(match.Groups[2].Value),
            double.Parse(match.Groups[3].Value)
        );
    }

    private static Tuple<double, double, double> GetRelativePhaseValues(double bias, string reply, Regex pattern)
    {
        var match = pattern.Match(reply);

        return Tuple.Create(
            int.Parse(match.Groups[1].Value) * bias / 20000.0,
            int.Parse(match.Groups[2].Value) * bias / 20000.0,
            int.Parse(match.Groups[3].Value) * bias / 20000.0
        );
    }

    /// <inheritdoc/>
    public async Task<MeasureOutput> GetActualValues()
    {
        var results = await Task.WhenAll(_device.Execute(
            SerialPortRequest.Create("BU", BuReply), // 0
            SerialPortRequest.Create("AU", AuReply), // 1
            SerialPortRequest.Create("BI", BiReply), // 2
            SerialPortRequest.Create("AI", AiReply), // 3
            SerialPortRequest.Create("AW", AwReply), // 4
            SerialPortRequest.Create("MP", MpReply), // 5
            SerialPortRequest.Create("MQ", MqReply), // 6
            SerialPortRequest.Create("MS", MsReply), // 7
            SerialPortRequest.Create("AF", AfReply)  // 8
        ));

        var voltageRange = double.Parse(BuReply.Match(results[0][^1]).Groups[1].Value);
        var currentRange = double.Parse(BiReply.Match(results[2][^1]).Groups[1].Value);

        var (voltage1, voltage2, voltage3) = GetRelativePhaseValues(voltageRange, results[1][^1], AuReply);
        var (current1, current2, current3) = GetRelativePhaseValues(currentRange, results[3][^1], AiReply);

        var (active1, active2, active3) = GetAbsolutePhaseValues(results[5][^1], MpReply);
        var (reactive1, reactive2, reactive3) = GetAbsolutePhaseValues(results[6][^1], MqReply);
        var (apparent1, apparent2, apparent3) = GetAbsolutePhaseValues(results[7][^1], MsReply);

        var frequency = double.Parse(AfReply.Match(results[8][^1]).Groups[1].Value);

        var angles = AwReply.Match(results[4][^1]);
        var voltage1Angle = double.Parse(angles.Groups[1].Value);
        var current1Angle = double.Parse(angles.Groups[2].Value);
        var voltage2Angle = double.Parse(angles.Groups[3].Value);
        var current2Angle = double.Parse(angles.Groups[4].Value);
        var voltage3Angle = double.Parse(angles.Groups[5].Value);
        var current3Angle = double.Parse(angles.Groups[6].Value);

        // [TODO] Phase order
        return new()
        {
            ActivePower = active1 + active2 + active3,
            ApparentPower = apparent1 + apparent2 + apparent3,
            ReactivePower = reactive1 + reactive2 + reactive3,
            Frequency = frequency,
            PhaseOrder = voltage2Angle < voltage3Angle ? "123" : "132",
            Phases = {
                new() {
                    ActivePower = active1,
                    AngleCurrent = current1Angle,
                    AngleVoltage = voltage1Angle,
                    ApparentPower = apparent1,
                    Current = current1,
                    PowerFactor = active1 == 0 ? null : active1/apparent1,
                    ReactivePower = reactive1,
                    Voltage = voltage1,
                }, new() {
                    ActivePower = active2,
                    AngleCurrent = current2Angle,
                    AngleVoltage = voltage2Angle,
                    ApparentPower = apparent2,
                    Current = current2,
                    PowerFactor = active2 == 0 ? null : active2/apparent2,
                    ReactivePower = reactive2,
                    Voltage = voltage2,
                }, new() {
                    ActivePower = active3,
                    AngleCurrent = current3Angle,
                    AngleVoltage = voltage3Angle,
                    ApparentPower = apparent3,
                    Current = current3,
                    PowerFactor = active3 == 0 ? null : active3/apparent3,
                    ReactivePower = reactive3,
                    Voltage = voltage3,
                }
            }
        };
    }
}
