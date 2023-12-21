using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace MeterTestSystemApi;

/// <summary>
/// 
/// </summary>
public class MeterTestSystemMock : IMeterTestSystem
{
    private readonly ILogger<SimulatedSource> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 
    /// </summary>
    public MeterTestSystemMock(ILogger<SimulatedSource> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        Source = new SimulatedSource(_logger, _configuration);
        RefMeter = new RefMeterMock(Source);
        ErrorCalculator = new ErrorCalculatorMock(Source);
    }

    /// <summary>
    /// 
    /// </summary>
    public AmplifiersAndReferenceMeter AmplifiersAndReferenceMeter => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public ISource Source { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public IRefMeter RefMeter { get; }

    /// <summary>
    /// 
    /// </summary>
    public IErrorCalculator ErrorCalculator { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeterTestSystemCapabilities> GetCapabilities()
    {
        return Task.FromResult(new MeterTestSystemCapabilities
        {
            SupportedCurrentAmplifiers = {
                CurrentAmplifiers.SCG1020,
                CurrentAmplifiers.VI201x0,
                CurrentAmplifiers.VI201x0x1,
                CurrentAmplifiers.VI201x1,
                CurrentAmplifiers.VI202x0,
                CurrentAmplifiers.VI202x0x1,
                CurrentAmplifiers.VI202x0x2,
                CurrentAmplifiers.VI202x0x5,
                CurrentAmplifiers.VI220,
                CurrentAmplifiers.VI221x0,
                CurrentAmplifiers.VI222x0,
                CurrentAmplifiers.VI222x0x1,
                CurrentAmplifiers.VUI301,
                CurrentAmplifiers.VUI302,
            },
            SupportedCurrentAuxiliaries = {
                CurrentAuxiliaries.V200,
                CurrentAuxiliaries.VI201x0,
                CurrentAuxiliaries.VI201x0x1,
                CurrentAuxiliaries.VI201x1,
                CurrentAuxiliaries.VI202x0,
                CurrentAuxiliaries.VI221x0,
                CurrentAuxiliaries.VI222x0,
                CurrentAuxiliaries.VUI301,
                CurrentAuxiliaries.VUI302,
            },
            SupportedReferenceMeters = {
                ReferenceMeters.COM3003,
                ReferenceMeters.COM3003x1x2,
                ReferenceMeters.COM5003x0x1,
                ReferenceMeters.COM5003x1,
                ReferenceMeters.EPZ303x1,
                ReferenceMeters.EPZ303x10,
                ReferenceMeters.EPZ303x10x1,
                ReferenceMeters.EPZ303x5,
                ReferenceMeters.EPZ303x8,
                ReferenceMeters.EPZ303x8x1,
                ReferenceMeters.EPZ303x9
            },
            SupportedVoltageAmplifiers = {
                VoltageAmplifiers.SVG3020,
                VoltageAmplifiers.VU211x0,
                VoltageAmplifiers.VU211x1,
                VoltageAmplifiers.VU211x2,
                VoltageAmplifiers.VU220,
                VoltageAmplifiers.VU220x01,
                VoltageAmplifiers.VU220x02,
                VoltageAmplifiers.VU220x03,
                VoltageAmplifiers.VU220x04,
                VoltageAmplifiers.VU221x0,
                VoltageAmplifiers.VU221x0x2,
                VoltageAmplifiers.VU221x0x3,
                VoltageAmplifiers.VU221x1,
                VoltageAmplifiers.VU221x2,
                VoltageAmplifiers.VU221x3,
                VoltageAmplifiers.VUI301,
                VoltageAmplifiers.VUI302,
            },
            SupportedVoltageAuxiliaries = {
                VoltageAuxiliaries.V210,
                VoltageAuxiliaries.VU211x0,
                VoltageAuxiliaries.VU211x1,
                VoltageAuxiliaries.VU211x2,
                VoltageAuxiliaries.VU221x0,
                VoltageAuxiliaries.VU221x1,
                VoltageAuxiliaries.VU221x2,
                VoltageAuxiliaries.VU221x3,
                VoltageAuxiliaries.VUI301,
                VoltageAuxiliaries.VUI302,
            }
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion()
    {
        return Task.FromResult(new MeterTestSystemFirmwareVersion()
        {
            ModelName = "DeviceMock",
            Version = "1.0"
        }
        );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeter settings)
    {
        throw new NotImplementedException();
    }
}
