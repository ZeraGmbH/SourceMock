using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefMeterApi.Actions.Device;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;

namespace MeterTestSystemApi;

/// <summary>
/// 
/// </summary>
/// <param name="source"></param>
/// <param name="refMeter"></param>
/// <param name="errorCalculatorMock"></param>
/// <param name="logger"></param>
/// <param name="configuration"></param>
public class MeterTestSystemAcMock(ISourceMock source, IMockRefMeter refMeter, IErrorCalculatorMock errorCalculatorMock, ILogger<SimulatedSource> logger, IConfiguration configuration) : IMeterTestSystem
{
    private readonly ILogger<SimulatedSource> _logger = logger;

    private readonly IConfiguration _configuration = configuration;

    /// <inheritdoc/>
    public event Action<ErrorConditions> ErrorConditionsChanged = null!;

    /// <summary>
    /// 
    /// </summary>
    public AmplifiersAndReferenceMeter GetAmplifiersAndReferenceMeter(IInterfaceLogger interfaceLogger) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public ISource Source { get; private set; } = source;

    /// <summary>
    /// 
    /// </summary>
    public IRefMeter RefMeter { get; } = refMeter;


    private readonly List<IErrorCalculator> _errorCalculators = [errorCalculatorMock];

    /// <summary>
    /// 
    /// </summary>
    public IErrorCalculator[] ErrorCalculators => _errorCalculators.ToArray();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeterTestSystemCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger) => Task.FromResult<MeterTestSystemCapabilities>(null!);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) =>
        Task.FromResult(new MeterTestSystemFirmwareVersion
        {
            ModelName = "DeviceMock",
            Version = "1.0"
        });

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task SetAmplifiersAndReferenceMeter(IInterfaceLogger logger, AmplifiersAndReferenceMeter settings) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ErrorConditions> GetErrorConditions(IInterfaceLogger logger)
    {
        var errors = new ErrorConditions { HasFuseError = DateTime.Now.Minute % 2 == 0 };

        ErrorConditionsChanged?.Invoke(errors);

        return Task.FromResult(errors);
    }
}
