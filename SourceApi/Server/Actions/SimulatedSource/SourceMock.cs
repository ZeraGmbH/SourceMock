using Microsoft.Extensions.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source;

public abstract class SourceMock : ISourceMock
{
    protected ILogger<SourceMock>? _logger;
    protected SourceCapabilities _sourceCapabilities;
    protected LoadpointInfo _info = new();
    protected DosageProgress _status = new();
    protected DateTime _startTime;
    protected double _dosageEnergy;
    protected bool _dosageMode = false;
    protected TargetLoadpoint? _loadpoint;
    protected ISourceCapabilityValidator _validator;

    public SourceMock(ILogger<SourceMock> logger, SourceCapabilities sourceCapabilities, ISourceCapabilityValidator validator)
    {
        _logger = logger;
        _sourceCapabilities = sourceCapabilities;
        _validator = validator;
    }

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> SetLoadpoint(TargetLoadpoint loadpoint)
    {
        var isValid = _validator.IsValid(loadpoint, _sourceCapabilities);

        if (isValid == SourceApiErrorCodes.SUCCESS)
        {
            _logger?.LogTrace("Loadpoint set, source turned on.");
            _loadpoint = loadpoint;

            _info.IsActive = true;
        }

        _dosageMode = false;

        _info.SavedAt = _info.ActivatedAt = DateTime.Now;

        return Task.FromResult(isValid);
    }

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> TurnOff()
    {
        _logger?.LogTrace("Source turned off.");

        _info.IsActive = false;

        return Task.FromResult(SourceApiErrorCodes.SUCCESS);
    }

    /// <inheritdoc/>
    public TargetLoadpoint? GetCurrentLoadpoint() => _loadpoint;

    public Task<SourceCapabilities> GetCapabilities() => Task.FromResult(_sourceCapabilities);

    public Task SetDosageMode(bool on)
    {
        _dosageMode = on;

        return Task.CompletedTask;
    }

    public Task SetDosageEnergy(double value, double meterConstant)
    {
        _dosageEnergy = value;

        return Task.CompletedTask;
    }

    public Task StartDosage()
    {
        _startTime = DateTime.Now;
        _status.Active = true;
        _dosageMode = false;

        return Task.CompletedTask;
    }

    public Task CancelDosage()
    {
        _status.Active = false;
        _status.Remaining = 0;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meterConstant"></param>
    /// <returns></returns>
    public abstract Task<DosageProgress> GetDosageProgress(double meterConstant);

    public Task<bool> CurrentSwitchedOffForDosage()
    {
        _logger?.LogTrace("Mock switches off the current for dosage");

        return Task.FromResult(_dosageMode);
    }

    public LoadpointInfo GetActiveLoadpointInfo() => _info;

    public bool Available => true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected double DosageDone()
    {
        _status.Active = false;

        _dosageMode = true;

        return _dosageEnergy;
    }
}

