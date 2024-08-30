using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source;

public interface ISourceMock : ISource
{
}

public interface IDosageMock : IDosage
{
    Task NoSource(IInterfaceLogger logger);
}

public abstract class SourceMock(ILogger<SourceMock> logger, SourceCapabilities sourceCapabilities, ISourceCapabilityValidator validator) : ISourceMock, IDosageMock
{
    protected bool UseLoadpoint = true;

    protected ILogger<SourceMock>? _logger = logger;
    protected SourceCapabilities _sourceCapabilities = sourceCapabilities;
    protected LoadpointInfo _info = new();
    protected DosageProgress _status = new();
    protected DateTime _startTime;
    protected ActiveEnergy _dosageEnergy;
    protected bool _dosageMode = false;
    protected TargetLoadpoint? _loadpoint;
    protected ISourceCapabilityValidator _validator = validator;

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        var isValid = _validator.IsValid(loadpoint, _sourceCapabilities);

        if (isValid == SourceApiErrorCodes.SUCCESS)
        {
            _logger?.LogTrace("Loadpoint set, source turned on.");
            _loadpoint = loadpoint;

            _info.SavedAt = _info.ActivatedAt = DateTime.Now;
            _info.IsActive = true;
        }

        _dosageMode = false;

        return Task.FromResult(isValid);
    }

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger)
    {
        _logger?.LogTrace("Source turned off.");

        _info.IsActive = false;

        return Task.FromResult(SourceApiErrorCodes.SUCCESS);
    }

    public Task<SourceCapabilities> GetCapabilities(IInterfaceLogger interfaceLogger) => Task.FromResult(_sourceCapabilities);

    public Task SetDosageMode(IInterfaceLogger logger, bool on)
    {
        _dosageMode = on;

        return Task.CompletedTask;
    }

    public Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant)
    {
        _dosageEnergy = value;

        return Task.CompletedTask;
    }

    public Task StartDosage(IInterfaceLogger logger)
    {
        _startTime = DateTime.Now;
        _status.Active = true;
        _dosageMode = false;

        return Task.CompletedTask;
    }

    public Task CancelDosage(IInterfaceLogger logger)
    {
        _status.Active = false;
        _status.Remaining = ActiveEnergy.Zero;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="meterConstant"></param>
    /// <returns></returns>
    public abstract Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant);

    public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger)
    {
        _logger?.LogTrace("Mock switches off the current for dosage");

        return Task.FromResult(_dosageMode);
    }

    public LoadpointInfo GetActiveLoadpointInfo(IInterfaceLogger interfaceLogger) => _info;

    public bool GetAvailable(IInterfaceLogger interfaceLogger) => true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected ActiveEnergy DosageDone()
    {
        _status.Active = false;

        _dosageMode = true;

        return _dosageEnergy;
    }

    public TargetLoadpoint? GetCurrentLoadpoint(IInterfaceLogger logger) => _loadpoint;

    public virtual Task NoSource(IInterfaceLogger logger)
    {
        UseLoadpoint = false;

        return Task.CompletedTask;
    }
}

