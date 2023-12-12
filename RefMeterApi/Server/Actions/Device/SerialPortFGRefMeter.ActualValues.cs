using System.Text.RegularExpressions;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

partial class SerialPortFGRefMeter
{
    /// <inheritdoc/>
    public async Task<MeasureOutput> GetActualValues()
    {
        TestConfigured();

        var afRequest = SerialPortRequest.Create("AF", new Regex(@"^AF(.+)$"));
        var aiRequest = SerialPortRequest.Create("AI", new Regex(@"^AIR(.{5})S(.{5})T(.{5})$"));
        var auRequest = SerialPortRequest.Create("AU", new Regex(@"^AUR(.{5})S(.{5})T(.{5})$"));
        var awRequest = SerialPortRequest.Create("AW", new Regex(@"^AWR(.{5})(.{5})S(.{5})(.{5})T(.{5})(.{5})$"));
        var biRequest = SerialPortRequest.Create("BI", new Regex(@"^BI(.+)$"));
        var buRequest = SerialPortRequest.Create("BU", new Regex(@"^BU(.+)$"));
        var mpRequest = SerialPortRequest.Create("MP", new Regex(@"^MPR([^;]+);S([^;]+);T([^;]+);$"));
        var mqRequest = SerialPortRequest.Create("MQ", new Regex(@"^MQR([^;]+);S([^;]+);T([^;]+);$"));
        var msRequest = SerialPortRequest.Create("MS", new Regex(@"^MSR([^;]+);S([^;]+);T([^;]+);$"));

        await Task.WhenAll(_device.Execute(afRequest, aiRequest, auRequest, awRequest, biRequest, buRequest, mpRequest, mqRequest, msRequest));

        var voltageRange = double.Parse(buRequest.EndMatch!.Groups[1].Value);
        var currentRange = double.Parse(biRequest.EndMatch!.Groups[1].Value);

        var voltage1 = int.Parse(auRequest.EndMatch!.Groups[1].Value) * voltageRange / 20000.0;
        var voltage2 = int.Parse(auRequest.EndMatch!.Groups[2].Value) * voltageRange / 20000.0;
        var voltage3 = int.Parse(auRequest.EndMatch!.Groups[3].Value) * voltageRange / 20000.0;

        var current1 = int.Parse(aiRequest.EndMatch!.Groups[1].Value) * currentRange / 20000.0;
        var current2 = int.Parse(aiRequest.EndMatch!.Groups[2].Value) * currentRange / 20000.0;
        var current3 = int.Parse(aiRequest.EndMatch!.Groups[3].Value) * currentRange / 20000.0;

        var active1 = double.Parse(mpRequest.EndMatch!.Groups[1].Value);
        var active2 = double.Parse(mpRequest.EndMatch!.Groups[2].Value);
        var active3 = double.Parse(mpRequest.EndMatch!.Groups[3].Value);

        var reactive1 = double.Parse(mqRequest.EndMatch!.Groups[1].Value);
        var reactive2 = double.Parse(mqRequest.EndMatch!.Groups[2].Value);
        var reactive3 = double.Parse(mqRequest.EndMatch!.Groups[3].Value);

        var apparent1 = double.Parse(msRequest.EndMatch!.Groups[1].Value);
        var apparent2 = double.Parse(msRequest.EndMatch!.Groups[2].Value);
        var apparent3 = double.Parse(msRequest.EndMatch!.Groups[3].Value);

        var frequency = double.Parse(afRequest.EndMatch!.Groups[1].Value);

        var voltage1Angle = double.Parse(awRequest.EndMatch!.Groups[1].Value);
        var current1Angle = double.Parse(awRequest.EndMatch!.Groups[2].Value);
        var voltage2Angle = double.Parse(awRequest.EndMatch!.Groups[3].Value);
        var current2Angle = double.Parse(awRequest.EndMatch!.Groups[4].Value);
        var voltage3Angle = double.Parse(awRequest.EndMatch!.Groups[5].Value);
        var current3Angle = double.Parse(awRequest.EndMatch!.Groups[6].Value);

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
                    PowerFactor = apparent1 == 0 ? null : active1/apparent1,
                    ReactivePower = reactive1,
                    Voltage = voltage1,
                }, new() {
                    ActivePower = active2,
                    AngleCurrent = current2Angle,
                    AngleVoltage = voltage2Angle,
                    ApparentPower = apparent2,
                    Current = current2,
                    PowerFactor = apparent2 == 0 ? null : active2/apparent2,
                    ReactivePower = reactive2,
                    Voltage = voltage2,
                }, new() {
                    ActivePower = active3,
                    AngleCurrent = current3Angle,
                    AngleVoltage = voltage3Angle,
                    ApparentPower = apparent3,
                    Current = current3,
                    PowerFactor = apparent3 == 0 ? null : active3/apparent3,
                    ReactivePower = reactive3,
                    Voltage = voltage3,
                }
            }
        };
    }
}
