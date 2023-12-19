using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions.Device;
using SourceApi.Actions.Source;

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
    public IRefMeter RefMeter { get; } = new RefMeterMock();

    /// <summary>
    /// 
    /// </summary>
    public IErrorCalculator ErrorCalculator { get; } = new ErrorCalculatorMock();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<MeterTestSystemCapabilities> GetCapabilities()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion()
    {
        throw new NotImplementedException();
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
