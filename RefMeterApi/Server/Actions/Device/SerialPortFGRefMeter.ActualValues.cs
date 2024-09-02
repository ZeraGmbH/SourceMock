using System.Text.RegularExpressions;
using RefMeterApi.Models;
using SerialPortProxy;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Actions.Device;

partial class SerialPortFGRefMeter
{

    /* Outstanding AME request - only works properly if the device instance is a singleton. */
    private readonly ResponseShare<MeasuredLoadpoint, IInterfaceLogger> _actualValues;

    /// <inheritdoc/>
    public async Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveCurrentPhase = -1)
    {
        await TestConfiguredAsync(logger);

        return Utils.ConvertFromDINtoIEC(LibUtils.DeepCopy(await _actualValues.ExecuteAsync(logger)), firstActiveCurrentPhase);
    }

    /// <summary>
    /// Begin reading the actual values - this may take some time.
    /// </summary>
    /// <returns>Task reading the actual values.</returns>
    /// <exception cref="ArgumentException">Reply from the device was not recognized.</exception>
    private async Task<MeasuredLoadpoint> CreateActualValueRequestAsync(IInterfaceLogger logger)
    {
        /* Request raw data from device. */
        var afRequest = SerialPortRequest.Create("AF", new Regex(@"^AF(.+)$"));
        var aiRequest = SerialPortRequest.Create("AI", new Regex(@"^AIR(.{5})S(.{5})T(.{5})$"));
        var auRequest = SerialPortRequest.Create("AU", new Regex(@"^AUR(.{5})S(.{5})T(.{5})$"));
        var awRequest = SerialPortRequest.Create("AW", new Regex(@"^AWR(.{5})(.{5})S(.{5})(.{5})T(.{5})(.{5})$"));
        var biRequest = SerialPortRequest.Create("BI", new Regex(@"^BI(.+)$"));
        var buRequest = SerialPortRequest.Create("BU", new Regex(@"^BU(.+)$"));
        var mpRequest = SerialPortRequest.Create("MP", new Regex(@"^MPR([^;]+);S([^;]+);T([^;]+);$"));
        var mqRequest = SerialPortRequest.Create("MQ", new Regex(@"^MQR([^;]+);S([^;]+);T([^;]+);$"));
        var msRequest = SerialPortRequest.Create("MS", new Regex(@"^MSR([^;]+);S([^;]+);T([^;]+);$"));

        await Task.WhenAll(_device.Execute(logger, afRequest, aiRequest, auRequest, awRequest, biRequest, buRequest, mpRequest, mqRequest, msRequest));

        /* Convert text representations to numbers. */
        var voltageRange = double.Parse(buRequest.EndMatch!.Groups[1].Value);
        var currentRange = double.Parse(biRequest.EndMatch!.Groups[1].Value);

        var voltage1 = int.Parse(auRequest.EndMatch!.Groups[1].Value) * voltageRange / 20000.0;
        var voltage2 = int.Parse(auRequest.EndMatch!.Groups[2].Value) * voltageRange / 20000.0;
        var voltage3 = int.Parse(auRequest.EndMatch!.Groups[3].Value) * voltageRange / 20000.0;

        var current1 = int.Parse(aiRequest.EndMatch!.Groups[1].Value) * currentRange / 20000.0;
        var current2 = int.Parse(aiRequest.EndMatch!.Groups[2].Value) * currentRange / 20000.0;
        var current3 = int.Parse(aiRequest.EndMatch!.Groups[3].Value) * currentRange / 20000.0;

        var active1 = new ActivePower(double.Parse(mpRequest.EndMatch!.Groups[1].Value));
        var active2 = new ActivePower(double.Parse(mpRequest.EndMatch!.Groups[2].Value));
        var active3 = new ActivePower(double.Parse(mpRequest.EndMatch!.Groups[3].Value));

        var reactive1 = new ReactivePower(double.Parse(mqRequest.EndMatch!.Groups[1].Value));
        var reactive2 = new ReactivePower(double.Parse(mqRequest.EndMatch!.Groups[2].Value));
        var reactive3 = new ReactivePower(double.Parse(mqRequest.EndMatch!.Groups[3].Value));

        var apparent1 = new ApparentPower(double.Parse(msRequest.EndMatch!.Groups[1].Value));
        var apparent2 = new ApparentPower(double.Parse(msRequest.EndMatch!.Groups[2].Value));
        var apparent3 = new ApparentPower(double.Parse(msRequest.EndMatch!.Groups[3].Value));

        var frequency = double.Parse(afRequest.EndMatch!.Groups[1].Value);

        var voltage1Angle = double.Parse(awRequest.EndMatch!.Groups[1].Value);
        var current1Angle = double.Parse(awRequest.EndMatch!.Groups[2].Value);
        var voltage2Angle = double.Parse(awRequest.EndMatch!.Groups[3].Value);
        var current2Angle = double.Parse(awRequest.EndMatch!.Groups[4].Value);
        var voltage3Angle = double.Parse(awRequest.EndMatch!.Groups[5].Value);
        var current3Angle = double.Parse(awRequest.EndMatch!.Groups[6].Value);

        var result = new MeasuredLoadpoint
        {
            ActivePower = active1 + active2 + active3,
            ApparentPower = apparent1 + apparent2 + apparent3,
            ReactivePower = reactive1 + reactive2 + reactive3,
            Frequency = new(frequency),
            PhaseOrder = voltage2Angle < voltage3Angle ? "123" : "132",
            Phases = {
                new() {
                    Current = new() {
                      AcComponent = new() {
                        Rms = new(current1),
                        Angle = new(current1Angle),
                      },
                    },
                    Voltage = new() {
                        AcComponent = new() {
                            Rms = new(voltage1),
                            Angle = new(voltage1Angle),
                        },
                    },
                    ActivePower = active1,
                    ApparentPower = apparent1,
                    PowerFactor = active1/apparent1,
                    ReactivePower = reactive1,
                }, new() {
                    Current = new() {
                      AcComponent = new() {
                        Rms = new(current2),
                        Angle = new(current2Angle),
                      },
                    },
                    Voltage = new() {
                        AcComponent = new() {
                            Rms = new(voltage2),
                            Angle = new(voltage2Angle),
                        },
                    },
                    ActivePower = active2,
                    ApparentPower = apparent2,
                    PowerFactor = active2/apparent2,
                    ReactivePower = reactive2,
                }, new() {
                    Current = new() {
                      AcComponent = new() {
                        Rms = new(current3),
                        Angle = new(current3Angle),
                      },
                    },
                    Voltage = new() {
                        AcComponent = new() {
                            Rms = new(voltage3),
                            Angle = new(voltage3Angle),
                        },
                    },
                    ActivePower = active3,
                    ApparentPower = apparent3,
                    PowerFactor = active3/apparent3,
                    ReactivePower = reactive3,
                }
            }
        };

        return result;
    }
}
